using server.DTO;
using server.Models;

namespace server.Services
{
    public interface IMedicalRecord
    {
        Task<MedicalRecord> AddMedicalRecord(int appointmentId, MedicalRecordDTO.PrescriptionRequest prescriptionRequest);
        Task<List<MedicalRecordDetail>> AddMedicalRecordDetail(int recordId, List<MedicalRecordDTO.MedicineDto> medicines);
        Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetMedicalRecordsForAdmin(List<int> appointmentIds);
        Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetMedicalRecords(List<int> appointmentIds);
        Task<List<MedicalRecordDTO.MedicalRecordBasic>> GetRecentMedicalRecords(List<int> appointmentIds);
        Task<MedicalRecordDTO.MedicalRecordBasic> GetMedicalRecordsByRecoredId(int recordId);
        Task<List<MedicalRecordDTO.MedicineDto>> GetRecordDetail(int recordId);

        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string orderId, string orderInfo, int amount);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
        Task<int> CalculateAmountFromRecordId(int recordId);
        Task<MedicalRecordDTO.CheckPaymentStatusResponse?> CheckPaymentStatusAsync(string orderId);
        Task<string> CreatePaymentUrl(HttpContext context, float amount, string orderId, string orderType, string orderDescription, string name);
        PaymentDTO.PaymentCallBack PaymentExecute(IQueryCollection collections);
    }
}