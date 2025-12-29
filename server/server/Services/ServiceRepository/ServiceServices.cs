using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Middleware;
using AutoMapper;
using server.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace server.Services
{
    public class ServiceServices : IService
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;
        public ServiceServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Lấy danh sách tất cả dịch vụ
        public async Task<List<ServiceDTO.ServiceDetail>> GetAllServices()
        {
            var services = await _context.Services.ToListAsync();
            var serviceDTOs = _mapper.Map<List<ServiceDTO.ServiceDetail>>(services);
             return serviceDTOs;
        }

        // Lấy dịch vụ theo tên
        public async Task<ServiceDTO.ServiceDetail> GetServiceByName(string serviceName)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceName == serviceName);
            var serviceDTOs = _mapper.Map<ServiceDTO.ServiceDetail>(service);
            return serviceDTOs;
        }

        public async Task<List<ServiceDTO.ServiceDetail>> GetServiceBySpecialty(string specialtyName)
        {
            var services = await _context.Services.Where(s => s.Specialties.Any(sp => sp.Name == specialtyName)).ToListAsync();

            var serviceDTOs = _mapper.Map<List<ServiceDTO.ServiceDetail>>(services);

            return serviceDTOs;
        }

        public async Task<Service> PostService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<List<Service>> GetRandomServices()
        {
            return await _context.Services.OrderBy(service => Guid.NewGuid()).Take(3).ToListAsync();
        }

        public async Task<Service?> GetById(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<Service> Create(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<bool> Update(int id, Service updatedService)
        {
            var existing = await _context.Services.FindAsync(id);
            if (existing == null) return false;

            existing.ServiceName = updatedService.ServiceName;
            existing.Description = updatedService.Description;
            existing.Price = updatedService.Price;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}