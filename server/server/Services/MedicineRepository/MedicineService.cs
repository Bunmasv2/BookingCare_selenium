using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Models;

namespace server.Services
{
    public class MedicineService : IMedicine
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public MedicineService(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MedicineDTO.MedicineBasic>> GetAllMedicines()
        {
            var medicines = await _context.Medicines.ToListAsync();
            var medicineDTOs = _mapper.Map<List<MedicineDTO.MedicineBasic>>(medicines);
            return medicineDTOs;
        }

        public async Task<List<MedicineDTO.MedicineBasic>> SearchMedicinesByName(string query)
        {
            query = query.ToLower();
            
            return await _context.Medicines
                .Where(m => m.MedicalName.ToLower().Contains(query))
                .Select(m => new MedicineDTO.MedicineBasic
                {
                    MedicineId = m.MedicineId,
                    MedicalName = m.MedicalName,
                    Unit = m.Unit
                })
                .Take(10) // Limit results to 10 suggestions
                .ToListAsync();
        }
    }
}