using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Services;
using server.Util;
using Server.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.OpenApi.Expressions;
using System.Text.Json;
using System.Security.Claims;

namespace Clinic_Management.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecords : ControllerBase
    {
        private readonly ClinicManagementContext _context;
        private readonly IMedicalRecord _medicalRecordService;
        private readonly IAppointment _appointmentService;
        private readonly IPatient _patientService;
        private readonly IDoctor _doctorService;
        private readonly IMedicine _medicineService;
        private readonly IConfiguration _configuration;

        public MedicalRecords(ClinicManagementContext context, IMedicalRecord medicalRecordService, IAppointment appointmentService, IPatient patientService, IDoctor doctorService, IMedicine medicineService, IConfiguration configuration)
        {
            _context = context;
            _medicalRecordService = medicalRecordService;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _medicineService = medicineService;
            _configuration = configuration;
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("{appointmentId}")]
        public async Task<ActionResult> AddMedicalRecord(int appointmentId, [FromBody] MedicalRecordDTO.PrescriptionRequest prescriptionRequest)
        {
            var appointment = await _appointmentService.GetAppointmentById(appointmentId)
                ?? throw new ErrorHandlingException("Không tìm thấy lịch hẹn!");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            if (appointment.DoctorId != parsedUserId)
            {
                throw new ErrorHandlingException(403, "Bạn không có quyền truy cập vào lịch hẹn này!");
            }

            if (prescriptionRequest.Medicines.Any(m => m.Dosage == 0.ToString()))
                throw new ErrorHandlingException(400, "Liều dùng không được bằng 0");

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            int hour = now.Hour;

            DateOnly appointmentDate = DateOnly.FromDateTime(appointment.AppointmentDate.Value);
            DateOnly today = DateOnly.FromDateTime(now);

            if (appointmentDate != today)
            {
                throw new ErrorHandlingException(400, "Chỉ được kê thuốc vào ngày khám!");
            }

            if (appointment.AppointmentTime == "Sáng" && (hour < 7 || hour > 12) ||
                appointment.AppointmentTime == "Chiều" && (hour < 13 || hour > 17))
            {
                throw new ErrorHandlingException(400, $"Hiện tại không nằm trong khung giờ kê thuốc cho buổi {appointment.AppointmentTime.ToLower()}. Vui lòng kê thuốc trong khoảng thời gian quy định.");
            }

            var record = await _medicalRecordService.AddMedicalRecord(appointmentId, prescriptionRequest)
                ?? throw new ErrorHandlingException(400, "Lỗi khi tạo toa thuốc");

            var recordDetail = await _medicalRecordService.AddMedicalRecordDetail(record.RecordId, prescriptionRequest.Medicines)
                ?? throw new ErrorHandlingException(400, "Lỗi khi tạo toa thuốc");

            var patient = await _patientService.GetPatientById(appointment.PatientId.Value)
                ?? throw new ErrorHandlingException(400, "Không tìm thấy bệnh nhân!");

            await _appointmentService.UpdateStatus(appointment, "Đã khám");

            return Ok(new { message = "Tạo toa thuốc thành công!" });
        }

        private async Task SendEmailForPatient(string Email, Appointment appointment, MedicalRecordDTO.PrescriptionRequest prescriptionRequest, int recordId)
        {
            var doctor = await _doctorService.GetDoctorById(appointment.DoctorId.Value) ?? throw new ErrorHandlingException(400, "Không tìm thấy bác sĩ!");

            var recordDetail = await _medicalRecordService.GetRecordDetail(recordId);

            if (recordDetail == null || recordDetail.Count() == 0)
            {
                throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");
            }

            string subject = "Toa thuốc của bạn từ phòng khám";

            string body = $@"
                <p>Bạn đã được bác sĩ <b>{doctor.UserName}</b> kê toa thuốc trong buổi khám ngày <b>{appointment.AppointmentDate:dd/MM/yyyy}</b>.</p>
                <p>Chẩn đoán bệnh: <b>{prescriptionRequest.Diagnosis}</b></p>
                <p>Hướng điều trị: <b>{prescriptionRequest.Treatment}</b></p>
                <h3>Chi tiết toa thuốc:</h3>
                <table border='1' cellpadding='8' cellspacing='0' style='border-collapse:collapse;'>
                    <tr>
                       <th>Tên thuốc</th>
                       <th>Liều lượng</th>
                       <th>Tần suất</th>
                       <th>Thời gian</th>
                       <th>Cách dùng</th>
                       <th>Giá</th>
                       <th>Tổng số lượng</th>
                    </tr>";

            foreach (var item in recordDetail)
            {
                body += $@"
                    <tr style='text-align: center;'>
                        <td>{item.MedicineName}</td>
                        <td>{item.Dosage} Lần / Ngày </td>
                        <td>{item.FrequencyPerDay} Lần  / {item.Unit}</td>
                        <td>{item.DurationInDays} Ngày</td>
                        <td>{item.Usage}</td>
                        <td>{item.Price}</td>
                        <td>{item.Quantity}</td>
                    </tr>";
            }

            body += $@"</table>
                <p>Lời dặn của bác sĩ: <b>{prescriptionRequest.Notes}</b></p>
                <br><p><i>Lưu ý: Vui lòng sử dụng thuốc đúng theo hướng dẫn và quay lại tái khám nếu cần.</i></p>
                <p>Chúc bạn mau hồi phục sức khỏe!</p>";

            await EmailUtil.SendEmailAsync(_configuration, Email, subject, body);
        }

        [Authorize(Roles = "patient")]
        [HttpGet("prescriptions")]
        public async Task<ActionResult> GetPrescriptions()
        {
            var userId = HttpContext.Items["UserId"].ToString();
            var parsedUserId = Convert.ToInt32(userId);

            var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bệnh nhân!");

            var appointments = await _appointmentService.GetAppointmentsId(patient.PatientId);

            var medicalRecords = await _medicalRecordService.GetMedicalRecords(appointments) ?? throw new ErrorHandlingException("Không tìm thấy bệnh nhân!"); ;

            return Ok(medicalRecords);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("prescriptions/patient")]
        public async Task<ActionResult> GetPrescriptionsByPatient()
        {
            // Lấy danh sách appointments duy nhất theo PatientId
            var distinctAppointments = await _context.MedicalRecords
                .Join(_context.Appointments,
                    mr => mr.AppointmentId,
                    ap => ap.AppointmentId,
                    (mr, ap) => ap)
                .GroupBy(a => a.PatientId)
                .Select(g => new
                {
                    PatientId = g.Key,
                    AppointmentId = g.OrderBy(a => a.AppointmentDate).Select(a => a.AppointmentId).FirstOrDefault()
                })
                .ToListAsync();

            if (distinctAppointments == null || !distinctAppointments.Any())
            {
                throw new ErrorHandlingException("Không tìm thấy bệnh nhân!");
            }

            var resultList = new List<object>();

            foreach (var item in distinctAppointments)
            {
                // Lấy đơn thuốc theo AppointmentId
                var records = await _medicalRecordService.GetMedicalRecordsForAdmin(new List<int> { item.AppointmentId });

                // Nếu có MedicalRecord thì mới lấy thông tin bệnh nhân
                if (records != null && records.Any())
                {
                    var patientDetail = await _patientService.GetPatientDetailByUserId(item.PatientId.Value);

                    resultList.Add(new
                    {
                        Patient = patientDetail,
                        Prescriptions = records
                    });
                }
            }

            return Ok(resultList);
        }


        [Authorize(Roles = "admin")]
        [HttpGet("search/{keyWord}")]
        public async Task<ActionResult> Search(string keyWord)
        {
            var keyword = Others.RemoveDiacritics(keyWord).ToLower();

            var distinctAppointments = await _context.MedicalRecords
                .Join(_context.Appointments,
                    mr => mr.AppointmentId,
                    ap => ap.AppointmentId,
                    (mr, ap) => ap)
                .GroupBy(a => a.PatientId)
                .Select(g => new
                {
                    PatientId = g.Key,
                    AppointmentId = g.OrderBy(a => a.AppointmentDate).Select(a => a.AppointmentId).FirstOrDefault()
                })
                .ToListAsync();

            if (distinctAppointments == null || !distinctAppointments.Any())
            {
                throw new ErrorHandlingException(404, "Không tìm thấy bệnh nhân!");
            }

            var resultList = new List<object>();

            foreach (var item in distinctAppointments)
            {
                if (!item.PatientId.HasValue) continue;

                // Lấy đơn thuốc theo từng appointment
                var records = await _medicalRecordService.GetMedicalRecords(new List<int> { item.AppointmentId });

                if (records != null && records.Any())
                {
                    var patientDetail = await _patientService.GetPatientDetailByUserId(item.PatientId.Value);
                    var dobString = patientDetail.DateOfBirth?.ToString("dd/MM/yyyy") ?? "";

                    // Lọc theo keyword trên nhiều trường
                    var isMatch = Others.RemoveDiacritics(patientDetail.UserName).Contains(keyWord, StringComparison.OrdinalIgnoreCase) ||
                                Others.RemoveDiacritics(patientDetail.Email).Contains(keyWord, StringComparison.OrdinalIgnoreCase) ||
                                Others.RemoveDiacritics(patientDetail.PhoneNumber ?? "").Contains(keyWord, StringComparison.OrdinalIgnoreCase) ||
                                Others.RemoveDiacritics(dobString).Contains(keyWord, StringComparison.OrdinalIgnoreCase);

                    var filteredPrescriptions = records.Where(mr =>
                        Others.RemoveDiacritics(mr.PatientName).Contains(keyWord, StringComparison.OrdinalIgnoreCase) ||
                        Others.RemoveDiacritics(mr.DoctorName).Contains(keyWord, StringComparison.OrdinalIgnoreCase) ||
                        Others.RemoveDiacritics(mr.Diagnosis).Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                    if (isMatch || filteredPrescriptions.Any())
                    {
                        resultList.Add(new
                        {
                            Patient = patientDetail,
                            Prescriptions = filteredPrescriptions
                        });
                    }
                }
            }

            return Ok(resultList);
        }


        [Authorize(Roles = "admin")]
        [HttpGet("prescriptions/patient/{patientId}")]
        public async Task<ActionResult<List<MedicalRecordDTO.MedicalRecordBasic>>> GetAllMedicalRecordByPatientId(
            int patientId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null,
            [FromQuery] string? serviceName = null, [FromQuery] string? status = null)
        {
            try
            {
                // Lấy tất cả appointmentId có PatientId == patientId
                var appointmentsQuery = _context.Appointments
                    .Include(mr => mr.Patient)
                    .Where(a => a.PatientId == patientId);

                // Lọc theo ngày nếu có
                if (startDate.HasValue)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.AppointmentDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    // Đặt thời gian là cuối ngày để so sánh chính xác
                    DateTime endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    appointmentsQuery = appointmentsQuery.Where(a => a.AppointmentDate <= endOfDay);
                }

                var appointmentIds = await appointmentsQuery
                    .Select(a => a.AppointmentId)
                    .ToListAsync();

                // Truy vấn đơn thuốc dựa trên danh sách appointmentId
                var medicalRecords = await _medicalRecordService.GetMedicalRecordsForAdmin(appointmentIds)
                                    ?? throw new ErrorHandlingException("Không tìm thấy bệnh nhân!");

                // Lọc thêm theo dịch vụ và trạng thái nếu có
                var filteredRecords = medicalRecords;

                if (!string.IsNullOrEmpty(serviceName))
                {
                    filteredRecords = filteredRecords.Where(r => r.ServiceName == serviceName).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    filteredRecords = filteredRecords.Where(r => r.Status == status).ToList();
                }
                var sortedRecords = filteredRecords
                    .OrderByDescending(r => r.AppointmentDate)
                    .ToList();

                return Ok(sortedRecords);

            }
            catch (Exception ex)
            {
                throw new ErrorHandlingException(500, $"Lỗi khi lấy đơn thuốc: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("details/{recordId}")]
        public async Task<ActionResult> GetMedicalRecordDetailByRecordId(int recordId)
        {
            var recordDetail = await _medicalRecordService.GetRecordDetail(recordId) ?? throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");
            var recorRecentDetail = await _medicalRecordService.GetMedicalRecordsByRecoredId(recordId) ?? throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");

            string body = $@"
                 <p>Bạn đã được bác sĩ <b>{recorRecentDetail.DoctorName}</b> kê toa thuốc trong buổi khám ngày <b>{recorRecentDetail.AppointmentDate:dd/MM/yyyy}</b>.</p>
                 <p>Chẩn đoán bệnh: <b>{recorRecentDetail.Diagnosis}</b></p>
                 <p>Hướng điều trị: <b>{recorRecentDetail.Treatment}</b></p>
                 <h3>Chi tiết toa thuốc:</h3>
                 <table border='1' cellpadding='8' cellspacing='0' style='border-collapse:collapse;'>
                     <tr>
                         <th>Tên thuốc</th>
                         <th>Liều dùng</th>
                         <th>Số lần/ngày</th>
                         <th>Số ngày</th>
                         <th>Cách dùng</th>
                         <th>Số lượng</th>
                         <th>Đơn vị thuốc</th>
                     </tr>";

            foreach (var item in recordDetail)
            {
                body += $@"
                     <tr style='text-align: center;'>
                         <td>{item.MedicineName}</td>
                         <td>{item.Dosage}</td>
                         <td>{item.FrequencyPerDay}</td>
                         <td>{item.DurationInDays}</td>
                         <td>{item.Usage}</td>
                         <td>X {item.Quantity}</td>
                         <td>{item.Unit}</td>
                     </tr>";
            }

            body += $@"</table>
                 <p>Lời dặn của bác sĩ: <b>{recorRecentDetail.Notes}</b></p>";
            return Content(body, "text/html");
        }

        [Authorize(Roles = "patient")]
        [HttpGet("prescriptions/recently")]
        public async Task<ActionResult> GetRecentPrescriptions()
        {
            var userId = HttpContext.Items["UserId"].ToString();
            var parsedUserId = Convert.ToInt32(userId);

            var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException(400, "Không tìm thấy bệnh nhân!");

            var appointments = await _appointmentService.GetAppointmentsId(patient.PatientId) ?? throw new ErrorHandlingException(400, "Không tìm thấy lịch hẹn!");

            var medicalRecords = await _medicalRecordService.GetRecentMedicalRecords(appointments) ?? throw new ErrorHandlingException("Không tìm thấy hồ sơ bệnh án!");

            return Ok(medicalRecords);
        }

        [Authorize(Roles = "patient, admin")]
        [HttpGet("detail/{recordId}")]
        public async Task<ActionResult> GetMedicalRecordDetail(int recordId)
        {
            // var userId = HttpContext.Items["UserId"].ToString();
            // var parsedUserId = Convert.ToInt32(userId);

            // var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bệnh nhân!");

            var recordDetail = await _medicalRecordService.GetRecordDetail(recordId) ?? throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");

            return Ok(recordDetail);
        }

        [HttpGet("recorddetail/{appointmentId}")]
        public async Task<ActionResult> GetMedicalRecordDetails(int appointmentId)
        {
            Console.WriteLine("AAAAAAAAAAAAA");

            var appointment = await _appointmentService.GetAppointmentById(appointmentId);
            if (appointment == null)
                return NotFound(new { ErrorMessage = "Không tìm thấy lịch hẹn!" });

            if (appointment.MedicalRecord == null)
                return NotFound(new { ErrorMessage = "Không có hồ sơ bệnh án nào được liên kết với lịch hẹn này!" });

            var recordDetail = await _medicalRecordService.GetRecordDetail(appointment.MedicalRecord.RecordId);
            if (recordDetail == null)
                throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");

            var recorRecentDetail = await _medicalRecordService.GetMedicalRecordsByRecoredId(appointment.MedicalRecord.RecordId);
            if (recorRecentDetail == null)
                throw new ErrorHandlingException("Không tìm thấy chi tiết toa thuốc!");

            string body = $@"
                <p>Bạn đã được bác sĩ <b>{recorRecentDetail.DoctorName}</b> kê toa thuốc trong buổi khám ngày <b>{recorRecentDetail.AppointmentDate:dd/MM/yyyy}</b>.</p>
                <p>Chẩn đoán bệnh: <b>{recorRecentDetail.Diagnosis}</b></p>
                <p>Hướng điều trị: <b>{recorRecentDetail.Treatment}</b></p>
                <h3>Chi tiết toa thuốc:</h3>
                <table border='1' cellpadding='8' cellspacing='0' style='border-collapse:collapse;'>
                    <tr>
                        <th>Tên thuốc</th>
                        <th>Liều dùng</th>
                        <th>Số lần/ngày</th>
                        <th>Số ngày</th>
                        <th>Cách dùng</th>
                        <th>Số lượng</th>
                        <th>Đơn vị thuốc</th>
                    </tr>";

            foreach (var item in recordDetail)
            {
                body += $@"
                    <tr style='text-align: center;'>
                        <td>{item.MedicineName}</td>
                        <td>{item.Dosage}</td>
                        <td>{item.FrequencyPerDay}</td>
                        <td>{item.DurationInDays}</td>
                        <td>{item.Usage}</td>
                        <td>X {item.Quantity}</td>
                        <td>{item.Unit}</td>
                    </tr>";
            }

            body += $@"</table>
                <p>Lời dặn của bác sĩ: <b>{recorRecentDetail.Notes}</b></p>";

            return Content(body, "text/html");
        }


        [HttpPost("create-vnpay/{recordId}")]
        public async Task<IActionResult> CreatePayment(int recordId)
        {
            Console.WriteLine($"Mã record: {recordId}");
            var appointment = await _context.Appointments
            .Include(a => a.Service)
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.MedicalRecord.RecordId == recordId);


            if (appointment == null)
                throw new ErrorHandlingException(404, "Không tìm thấy lịch hẹn.");
            if (appointment.Status != "Đã khám")
                throw new ErrorHandlingException(400, "Lịch hẹn chưa hoàn thành.");
            else
            {
                int totalAmount = await _medicalRecordService.CalculateAmountFromRecordId(recordId);

                // Bạn set sẵn ở backend
                string orderType = "other";
                string orderDescription = "thanh toán đơn thuốc";
                string name = $"{appointment.Patient?.User?.FullName ?? "Unknown"}";


                Console.WriteLine($"Mã record: {appointment.Patient?.User?.FullName ?? "Unknown"}");

                var paymentUrl = await _medicalRecordService.CreatePaymentUrl(HttpContext, totalAmount, recordId.ToString(), orderType, orderDescription, name);
                if (paymentUrl == null)
                {
                    throw new ErrorHandlingException(500, "Không thể tạo URL thanh toán");
                }
                return Ok(new { paymentUrl });
            }
        }
        public async Task SendEmailPayment(PaymentDTO.PaymentInformationModel paymentInfo, string Email)
        {
            var subject = "Xác nhận thanh toán dịch vụ khám bệnh";

            var body = $@"
                        <h2>Thông tin thanh toán</h2>
                        <p><strong>Họ tên bệnh nhân:</strong> {paymentInfo.Name}</p>
                        <p><strong>Mô tả giao dịch:</strong> {paymentInfo.OrderDescription}</p>
                        <p><strong>Số tiền:</strong> {paymentInfo.Amount} VNĐ</p>
                        <p><strong>Thời gian thanh toán:</strong> {paymentInfo.Date.ToString("dd/MM/yyyy HH:mm:ss")}</p>
                        <p>Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!</p>";
            await EmailUtil.SendEmailAsync(_configuration, Email, subject, body);
        }

        [HttpGet("callback")]
        public async Task<PaymentDTO.PaymentInformationModel> PaymentCallbackVnpay()
        {
            var response = _medicalRecordService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00" && response.TransactionCode == "00")
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Include(a => a.MedicalRecord)
                    .FirstOrDefaultAsync(a => a.MedicalRecord.RecordId == int.Parse(response.PaymentId));


                if (appointment == null)
                {
                    throw new ErrorHandlingException(500, "Không tìm thấy lịch hẹn");
                }

                string email = appointment?.Patient?.User?.Email ?? throw new ErrorHandlingException(400, "Không tìm thấy email bệnh nhân!");

                var oldStatus = appointment.Status;
                appointment.Status = "Đã hoàn thành";
                await _context.SaveChangesAsync();
                PaymentDTO.PaymentInformationModel paymentInfo = new PaymentDTO.PaymentInformationModel
                {
                    PaymentId = response.TransactionNo,
                    Amount = response.Amount,
                    Success = "success",
                    Name = appointment?.Patient?.User?.FullName ?? "Không xác định",
                    OrderDescription = response.PaymentInfo,
                    Date = response.PaymentDateTime
                };
                await SendEmailPayment(paymentInfo, email);
                return paymentInfo;
            }
            else
            {
                return new PaymentDTO.PaymentInformationModel
                {
                    PaymentId = "",
                    Amount = "0",
                    Success = "fail",
                    OrderDescription = "",
                    Name = "",
                    Date = DateTime.MinValue
                };
            }
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] MedicalRecordDTO.CreatePaymentRequest request)
        {
            Console.WriteLine("Mã toa thuốc : " + request.RecordId.ToString());
            if (request == null || string.IsNullOrWhiteSpace(request.OrderInfo))
            {
                return BadRequest("Invalid request");
            }
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.MedicalRecord.RecordId == request.RecordId);


            if (appointment == null)
                throw new Exception("Không tìm thấy lịch hẹn.");
            if (appointment.Status != "Đã khám")
                throw new Exception("Lịch hẹn chưa hoàn thành.");

            // Generate OrderId and Amount on the backend
            var orderId = Guid.NewGuid().ToString();
            int amount = await _medicalRecordService.CalculateAmountFromRecordId(request.RecordId);

            Console.WriteLine($"Generated OrderId: {orderId}");
            Console.WriteLine($"OrderInfo: {request.OrderInfo}");
            Console.WriteLine($"Generated Amount: {amount}");

            var result = await _medicalRecordService.CreatePaymentAsync(orderId, request.OrderInfo, amount);
            Console.WriteLine($"MoMo payment response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }


        [HttpGet("payment-execute")]
        public IActionResult PaymentExecute()
        {
            var data = _medicalRecordService.PaymentExecuteAsync(Request.Query);
            return Ok(data);
        }
        // [HttpGet("payment-status")]
        // public async Task<IActionResult> CheckPaymentStatus(string orderId)
        // {
        //     var result = await _momoService.CheckPaymentStatusAsync(orderId);
        //     if (result.IsSuccess)
        //     {
        //         return Ok(new { status = "success" });
        //     }
        //     else
        //     {
        //         return Ok(new { status = "failure" });
        //     }
        // }

        [HttpGet("check-payment-status")]
        public async Task<IActionResult> CheckPaymentStatus([FromQuery] string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest(new { message = "Thiếu mã đơn hàng (orderId)" });
            }

            // Gọi service kiểm tra trạng thái thanh toán MoMo
            var result = await _medicalRecordService.CheckPaymentStatusAsync(orderId);

            if (result == null)
            {
                return StatusCode(500, new { message = "Không thể kiểm tra trạng thái thanh toán từ MoMo" });
            }

            // Nếu thanh toán thành công (ResultCode == 0 theo tài liệu MoMo)
            if (result.ResultCode == 0)
            {
                // Tìm lịch hẹn liên kết với MedicalRecord.RecordId == orderId
                var appointment = await _context.Appointments
                    .Include(a => a.Patient).ThenInclude(p => p.User)
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .Include(a => a.Service)
                    .Include(a => a.MedicalRecord) // cần Include để truy xuất RecordId
                    .FirstOrDefaultAsync(a => a.MedicalRecord != null && a.MedicalRecord.RecordId == int.Parse(orderId));

                if (appointment == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn tương ứng với đơn hàng" });
                }

                // Cập nhật trạng thái lịch hẹn
                string oldStatus = appointment.Status;
                appointment.Status = "Hoàn thành"; // hoặc bất kỳ giá trị chuẩn nào bạn đặt
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Đã thanh toán thành công",
                    paymentStatus = "Paid",
                    appointmentId = appointment.AppointmentId,
                    oldStatus = oldStatus,
                    updatedStatus = appointment.Status
                });
            }
            else
            {
                return Ok(new
                {
                    message = "Chưa thanh toán hoặc giao dịch thất bại",
                    paymentStatus = "Unpaid",
                    resultCode = result.ResultCode,
                    resultMessage = result.Message
                });
            }
        }

        [HttpPost("pay-by-cash")]
        public async Task<IActionResult> PaymentByCash([FromBody] CashDTO paymentCash)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.MedicalRecord.RecordId == paymentCash.RecordId);

            if (appointment == null)
                throw new ErrorHandlingException(404, "Không tìm thấy lịch hẹn.");
            if (appointment.Status != "Đã khám")
                throw new ErrorHandlingException(400, "Lịch hẹn chưa hoàn thành.");

            int totalAmount = await _medicalRecordService.CalculateAmountFromRecordId(paymentCash.RecordId);
            int total = totalAmount + paymentCash.CashBalance;

            if (total != paymentCash.Amount)
                throw new ErrorHandlingException(400, "Thanh toán tiền mặt sai.");

            string oldStatus = appointment.Status;
            appointment.Status = "Đã hoàn thành";
            await _context.SaveChangesAsync();

            return Ok("success");
        }

        [HttpGet("total-cash{recordId}")]
        public async Task<IActionResult> TotalCash(int recordId)
        {
            int total = await _medicalRecordService.CalculateAmountFromRecordId(recordId);
            return Ok(total);
        }
    }
}