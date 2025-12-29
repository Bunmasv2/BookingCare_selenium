using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Middleware;
using server.Models;
using static server.DTO.UserDTO;

namespace server.Services
{
    public class UserServices : IUser
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserServices(ClinicManagementContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserDTO.UserBasic> GetUserById(int id, string role)
        {
            ApplicationUser user;

            switch (role)
            {
                case "admin":
                    user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
                    break;

                case "doctor":
                    user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
                    break;

                case "patient":
                    user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
                    break;

                default:
                    throw new ErrorHandlingException(403, "Bạn không có quyền truy cập!");
            }

            var userDTO = _mapper.Map<UserDTO.UserBasic>(user);

            return userDTO;
        }

        public async Task<List<UserDTO.UserBasic>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDTO.UserBasic>>(users);
        }

        public async Task<List<UserDTO.Doctor>> GetDoctors()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("doctor");
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var doctors = await _context.Users
                .Include(u => u.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

            var doctorDTOs = _mapper.Map<List<UserDTO.Doctor>>(doctors);
            return doctorDTOs;
        }

        public async Task<List<UserDTO.Patient>> GetPatients()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("patient");
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var patients = await _context.Users
                .Include(u => u.Patient)
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

            var patientDTOs = _mapper.Map<List<UserDTO.Patient>>(patients);

            return patientDTOs;
        }

        public async Task<List<UserDTO.Admin>> GetAdmins()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("admin");
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var admins = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

            var adminDTOs = _mapper.Map<List<UserDTO.Admin>>(admins);

            return adminDTOs;
        }

        public async Task<List<ApplicationUser>> SearchUser(string role, string searchTerm)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);

            var filteredUsers = usersInRole
                .Where(u => u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return filteredUsers;
        }

        public async Task<List<UserDTO.Patient>> SearchUserByKeyWord(string keyWord)
        {
            if (string.IsNullOrWhiteSpace(keyWord))
            {
                return new List<UserDTO.Patient>();
            }

            keyWord = keyWord.Trim().ToLower();
            var users = await _context.Users
            .Include(u => u.Patient)
            .Where(p =>
                (!string.IsNullOrEmpty(p.FullName) && p.FullName.ToLower().Contains(keyWord)) ||
                (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(keyWord)) ||
                (!string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber.ToLower().Contains(keyWord)) ||
                (!string.IsNullOrEmpty(p.Address) && p.Address.ToLower().Contains(keyWord))
            )
            .ToListAsync();

            return _mapper.Map<List<UserDTO.Patient>>(users);
        }
    }
}