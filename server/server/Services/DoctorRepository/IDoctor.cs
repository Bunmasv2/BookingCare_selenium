using server.DTO;
using server.Models;

namespace server.Services
{
    public interface IDoctor
    {
        Task<List<DoctorDTO.DoctorBasic>> GetAllDoctors();
        Task<DoctorDTO.DoctorDetail> GetDoctorByName(string doctorName);
        Task<List<DoctorDTO.DoctorBasic>> GetDoctorsBySpecialty(string specialtyName);
        Task<List<DoctorDTO.DoctorBasic>> SearchDoctors(string keyword);
        Task<DoctorDTO.DoctorDetail> GetDoctorById(int doctorId);
        Task<List<DoctorDTO.DoctorSalaryDTO>> GetDoctorSalariesAsync(DateTime month);
        Task<SalarySummaryDTO> GetSalarySummaryAsync(DateTime month);
        Task<DoctorDTO.DoctorSalaryDetailResultDTO> GetDoctorSalaryDetailsAsync(int doctorId, DateTime month);
        Task<Dictionary<int, decimal>> CalculateTopDoctorBonusesAsync(DateTime month);
        // Task<int> GetTotalDoctorsAsync();
        Task<PaginatedResult<DoctorDTO.DoctorBasic>> GetDoctorsPaged(int pageNumber, string specialty = null, string searchKeyword = null);
    }
}
