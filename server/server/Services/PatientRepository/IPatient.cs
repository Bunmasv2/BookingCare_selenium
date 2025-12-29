using server.DTO;
using server.Models;

namespace server.Services
{
    public interface IPatient
    {
        // Task<List<PatientDTO.PatientBasic>> GetAllPatients();
        Task<PatientDTO.PatientBasic> GetPatientById(int patientId);
        //Task<PatientDTO.PatientDetail> GetPatientById(int patientId);
        Task<PatientDTO.PatientDetail> GetPatientByUserId(int userId);
        Task<List<PatientDTO.PatientDetail>> GetAllPatients();
        Task<PatientDTO.PatientDetail> GetPatientDetailByUserId(int patientId);
    }
}
