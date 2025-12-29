using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Util;

namespace server.Services
{
    public class MedicalRecordService : IMedicalRecord
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;
        private readonly MomoOptionModel _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MedicalRecordService> _logger;
        private readonly IConfiguration _configuration;

        public MedicalRecordService(ClinicManagementContext context, IMapper mapper, IConfiguration configuration, ILogger<MedicalRecordService> logger, IOptions<MomoOptionModel> options)
        {
            _options = options.Value;
            _httpClient = new HttpClient();
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<MedicalRecord> AddMedicalRecord(int appointmentId, MedicalRecordDTO.PrescriptionRequest prescriptionRequest)
        {
            var medicalRecord = new MedicalRecord
            {
                AppointmentId = appointmentId,
                Diagnosis = prescriptionRequest.Diagnosis,
                Treatment = prescriptionRequest.Treatment,
                Notes = prescriptionRequest.Notes,
                CreatedAt = DateTime.Now
            };

            await _context.MedicalRecords.AddAsync(medicalRecord);
            await _context.SaveChangesAsync();

            return medicalRecord;
        }

        public async Task<List<MedicalRecordDetail>> AddMedicalRecordDetail(int recordId, List<MedicalRecordDTO.MedicineDto> medicines)
        {
            var medicalRecordDetail = medicines.Select(medicine => new MedicalRecordDetail
            {
                ReCordId = recordId,
                MedicineId = medicine.MedicineId,
                Quantity = medicine.Quantity,
                Dosage = Int32.Parse(medicine.Dosage),
                FrequencyPerDay = Int32.Parse(medicine.FrequencyPerDay),
                DurationInDays = Int32.Parse(medicine.DurationInDays),
                Usage = medicine.Usage
            }).ToList();

            await _context.MedicalRecordDetails.AddRangeAsync(medicalRecordDetail);
            await _context.SaveChangesAsync();

            return medicalRecordDetail;
        }

        public async Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetMedicalRecords(List<int> appointmentIds)
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Doctor.User)
                .Include(mr => mr.Appointment.Patient.User)
                .Include(mr => mr.Appointment.Doctor.Specialty)
                .Include(mr => mr.Appointment.Service)
                .Where(mr => appointmentIds.Contains(mr.AppointmentId ?? 0) && mr.Appointment.Status == "Đã hoàn thành")
                .OrderBy(mr => mr.Appointment.AppointmentDate)
                .ToListAsync() ?? throw new ErrorHandlingException("Lỗi khi lấy danh sách toa thuốc!");

            var medicalRecordDTOs = _mapper.Map<List<MedicalRecordDTO.MedicalRecordBasic>>(medicalRecords);

            return medicalRecordDTOs;  
        }

        public async Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetMedicalRecordsForAdmin(List<int> appointmentIds)
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Doctor.User)
                .Include(mr => mr.Appointment.Patient.User)
                .Include(mr => mr.Appointment.Doctor.Specialty)
                .Include(mr => mr.Appointment.Service)
                .Where(mr => appointmentIds.Contains(mr.AppointmentId ?? 0))
                .OrderBy(mr => mr.Appointment.AppointmentDate)
                .ToListAsync() ?? throw new ErrorHandlingException("Lỗi khi lấy danh sách toa thuốc!");

            var medicalRecordDTOs = _mapper.Map<List<MedicalRecordDTO.MedicalRecordBasic>>(medicalRecords);

            return medicalRecordDTOs;  
        }

        public async Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetRecentMedicalRecords(List<int> appointmentIds)
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Doctor.User)
                .Include(mr => mr.Appointment.Patient.User)
                .Include(mr => mr.Appointment.Doctor.Specialty)
                .Include(mr => mr.Appointment.Service)
                .Where(mr => appointmentIds.Contains(mr.AppointmentId ?? 0) && mr.Appointment.Status == "Đã hoàn thành")
                .OrderBy(mr => mr.Appointment.AppointmentDate)
                .Take(3)
                .ToListAsync();
            
            var medicalRecordDTOs = _mapper.Map<List<MedicalRecordDTO.MedicalRecordBasic>>(medicalRecords);

            return medicalRecordDTOs;  
        }

        public async Task<MedicalRecordDTO.MedicalRecordBasic> GetMedicalRecordsByRecoredId(int recordId) {
            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Doctor.User)
                .Include(mr => mr.Appointment.Patient.User)
                .Include(mr => mr.Appointment.Doctor.Specialty)
                .Where(mr => mr.RecordId == recordId)
                .OrderBy(mr => mr.Appointment.AppointmentDate)
                .FirstOrDefaultAsync(); 

            var medicalRecordDTO = _mapper.Map<MedicalRecordDTO.MedicalRecordBasic>(medicalRecord);
            return medicalRecordDTO;
        }

        public async Task<List<MedicalRecordDTO.MedicineDto>> GetRecordDetail(int recordId)
        {
            var medicines = await _context.MedicalRecordDetails
                .Include(mr => mr.Medicine)
                .Where(mr => mr.ReCordId == recordId)
                .ToListAsync();

            var medicineDTOs = _mapper.Map<List<MedicalRecordDTO.MedicineDto>>(medicines);

            return medicineDTOs;
        }

        public async Task<string> CreatePaymentUrl(HttpContext context, float amount, string recordId, string orderType, string orderDescription, string name)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = timeNow.Ticks.ToString();

            var pay = new VnPayUtil();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{name} {orderDescription}");
            pay.AddRequestData("vnp_OrderType", orderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", recordId);

            var paymentUrl = pay.CreateRequestUrl(
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:HashSecret"]
            );

            _logger.LogInformation("Generated Payment URL: {PaymentUrl}", paymentUrl);

            return paymentUrl;
        }

        public PaymentDTO.PaymentCallBack PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayUtil();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            _logger.LogInformation("Response from VNPAY: {@Response}", response);

            return response;
        }
       public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string orderId, string orderInfo, int amount)
        {
            var requestId = Guid.NewGuid().ToString(); // NEW

            var rawData = $"accessKey={_options.AccessKey}&amount={amount}&extraData=&ipnUrl={_options.NotifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={_options.PartnerCode}&redirectUrl={_options.ReturnUrl}&requestId={requestId}&requestType={_options.RequestType}";
            var signature = ComputeHmacSha256(rawData, _options.SecretKey);

            var requestBody = new
            {
                partnerCode = _options.PartnerCode,
                accessKey = _options.AccessKey,
                requestId = requestId,
                amount = amount, // NO toString
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = _options.ReturnUrl,
                ipnUrl = _options.NotifyUrl,
                lang = "vi",
                extraData = "",
                requestType = _options.RequestType,
                signature = signature
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_options.MomoApiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseContent);

            return data;
        }


        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            return new MomoExecuteResponseModel
            {
                Amount = collection["amount"],
                OrderId = collection["orderId"],
                OrderInfo = collection["orderInfo"]
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    public async Task<int> CalculateAmountFromRecordId(int recordId)
        {
            Console.WriteLine("Mã toa thuốc service : " + recordId);
            var record = await _context.MedicalRecords
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Service)
                .Include(r => r.MedicalRecordDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(r => r.RecordId == recordId);

            if (record == null) return 0;

            float total = 0;
            Console.WriteLine("Tên dịch vụ : " + record.Appointment?.Service.ServiceName + "Giá dịch vụ: "+ record.Appointment?.Service?.Price);
            // 1. Giá dịch vụ khám
            if (record.Appointment?.Service?.Price != null)
            {
                total += record.Appointment.Service.Price.Value;
            }

            // 2. Tính tổng giá thuốc theo từng chi tiết
            foreach (var detail in record.MedicalRecordDetails)
            {
                if (detail.Medicine != null && detail.Medicine.Price != null && detail.Quantity != null)
                {
                    total += detail.Medicine.Price.Value * detail.Quantity.Value;
                }
            }

            return (int)total;
        }

        public async Task<MedicalRecordDTO.CheckPaymentStatusResponse?> CheckPaymentStatusAsync(string orderId)
        {
            string partnerCode = _configuration["Momo:PartnerCode"];
            string accessKey = _configuration["Momo:AccessKey"];
            string secretKey = _configuration["Momo:SecretKey"];
            string endpoint = _configuration["Momo:QueryUrl"]; // VD: https://test-payment.momo.vn/v2/gateway/api/query

            var requestId = Guid.NewGuid().ToString();

            // Tạo raw data để ký
            var rawData = $"accessKey={accessKey}&orderId={orderId}&partnerCode={partnerCode}&requestId={requestId}";
            string signature = CreateSignature(rawData, secretKey);

            // Payload gửi đến API truy vấn
            var payload = new
            {
                partnerCode = partnerCode,
                requestId = requestId,
                orderId = orderId,
                lang = "vi",
                signature = signature
            };

            // Chuyển payload thành JSON
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Gửi POST request
            var response = await _httpClient.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Đọc nội dung phản hồi
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<MedicalRecordDTO.CheckPaymentStatusResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }


        private string CreateSignature(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }

        // public async Task<MomoPaymentStatusResponseModel> CheckPaymentStatusAsync(string orderId)
        // {
        //     var requestBody = new
        //     {
        //         partnerCode = _options.PartnerCode,
        //         accessKey = _options.AccessKey,
        //         orderId = orderId,
        //         signature = ComputeHmacSha256($"partnerCode={_options.PartnerCode}&orderId={orderId}", _options.SecretKey)
        //     };

        //     var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        //     var response = await _httpClient.PostAsync(_options.MomoStatusApiUrl, content);

        //     var responseContent = await response.Content.ReadAsStringAsync();
        //     return JsonConvert.DeserializeObject<MomoPaymentStatusResponseModel>(responseContent);
        // }
    }
}