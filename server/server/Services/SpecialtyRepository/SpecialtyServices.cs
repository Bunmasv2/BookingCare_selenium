using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Models;

namespace server.Services
{
    public class SpecialtyServices : ISpecialty
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public SpecialtyServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<SpecialtyDTO>> GetSpecialties()
        {
            var specialties = await _context.Specialties.ToListAsync();
            var specialtyDTOs = _mapper.Map<List<SpecialtyDTO>>(specialties);

            return specialtyDTOs;
        }


        public async Task<SpecialtyDTO?> GetDescription(string specialty)
        {
            Specialty description = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == specialty);
            SpecialtyDTO specialtyDTO = _mapper.Map<SpecialtyDTO>(description);

            return specialtyDTO;
        }
        
        public async Task<List<Specialty>> GetRandomSpecialties()
        {
            return await _context.Specialties.OrderBy(specialty => Guid.NewGuid()).Take(3).ToListAsync();

        }
        
        public async Task<Specialty?> GetById(int id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<Specialty> Create(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<bool> Update(int id, Specialty updatedSpecialty)
        {
            var existing = await _context.Specialties.FindAsync(id);
            if (existing == null) return false;

            existing.Name = updatedSpecialty.Name;
            existing.Description = updatedSpecialty.Description;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return false;

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Specialty> GetSpecialty(string name)
        {
            var specialty = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == name);
            return specialty;
        }
    }
}
