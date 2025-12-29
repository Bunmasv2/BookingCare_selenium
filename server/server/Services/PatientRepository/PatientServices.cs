using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Models;

namespace server.Services
{
    public class PatientServices : IPatient
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public PatientServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        
        }
        public async Task<PatientDTO.PatientBasic> GetPatientById(int patientId)
        {
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(d => d.PatientId == patientId);

            var patientDTO = _mapper.Map<PatientDTO.PatientBasic>(patient);

            return patientDTO;
        }

        public async Task<PatientDTO.PatientDetail> GetPatientByUserId(int userId)
        {
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

            var patientDTO = _mapper.Map<PatientDTO.PatientDetail>(patient);

            return patientDTO;
        }

        public async Task<PatientDTO.PatientDetail> GetPatientDetailByUserId(int patientId)
        {
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientId == patientId);

            var patientDTO = _mapper.Map<PatientDTO.PatientDetail>(patient);

            return patientDTO;
        }

        public async Task<List<PatientDTO.PatientDetail>> GetAllPatients(){
            var patients = await _context.Patients.ToListAsync();
            return _mapper.Map<List<PatientDTO.PatientDetail>>(patients);
        }

        // public Task<PatientDTO.PatientDetail> CreatePatient(PatientDTO.PatientDetail patientDTO)
        // {
        //     throw new NotImplementedException();
        // }

        // public Task<bool> DeletePatient(int patientId)
        // {
        //     throw new NotImplementedException();
        // }

        // public Task<List<PatientDTO.PatientBasic>> GetAllPatients()
        // {
        //     throw new NotImplementedException();
        // }
    }
}