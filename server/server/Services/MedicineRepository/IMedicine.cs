using server.DTO;

namespace server.Services
{
    public interface IMedicine
    {
        Task<List<MedicineDTO.MedicineBasic>> GetAllMedicines();
        Task<List<MedicineDTO.MedicineBasic>> SearchMedicinesByName(string query);
    }
}