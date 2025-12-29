using server.Models;
using server.DTO;

namespace server.Services
{
    public interface IService
    {
        Task<List<ServiceDTO.ServiceDetail>> GetAllServices();
        Task<ServiceDTO.ServiceDetail> GetServiceByName(string serviceName);
        Task<List<ServiceDTO.ServiceDetail>> GetServiceBySpecialty(string specialtyName);
        Task<List<Service>> GetRandomServices();
        Task<Service?> GetById(int id);
        Task<Service> Create(Service service);
        Task<bool> Update(int id, Service updatedService);
        Task<bool> Delete(int id);
    }
}