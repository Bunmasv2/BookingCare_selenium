    using server.DTO;
    using server.Models;

    namespace server.Services
    {
    public interface IUser
    {
        Task<UserDTO.UserBasic> GetUserById(int id, string role);
        Task<List<UserDTO.UserBasic>> GetUsers();
        Task<List<UserDTO.Doctor>> GetDoctors();
        Task<List<UserDTO.Patient>> GetPatients();
        Task<List<UserDTO.Admin>> GetAdmins();
        Task<List<ApplicationUser>> SearchUser(string role, string searchTerm);
        Task<List<UserDTO.Patient>> SearchUserByKeyWord(string keyWord);
        }
    }
