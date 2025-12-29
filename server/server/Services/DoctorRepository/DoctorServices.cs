using Microsoft.EntityFrameworkCore;
using server.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using server.DTO;

namespace server.Services
{
    public class DoctorServices : IDoctor
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;
        public DoctorServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<DoctorDTO.DoctorBasic>> GetAllDoctors()
        {
            var doctors = await _context.Doctors.Include(doctor => doctor.User).ToListAsync();

            var doctorDTOs = _mapper.Map<List<DoctorDTO.DoctorBasic>>(doctors);
            return doctorDTOs;
        }

        public async Task<DoctorDTO.DoctorDetail> GetDoctorByName(string doctorName)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.User.FullName == doctorName);

            var doctorDTO = _mapper.Map<DoctorDTO.DoctorDetail>(doctor);

            return doctorDTO;
        }

        public async Task<List<DoctorDTO.DoctorBasic>> GetDoctorsBySpecialty(string specialtyName)
        {
            var doctors = await _context.Doctors.Include(doctor => doctor.User).Where(d => d.Specialty.Name == specialtyName).ToListAsync();

            var doctorDTOs = _mapper.Map<List<DoctorDTO.DoctorBasic>>(doctors);

            return doctorDTOs;
        }

        public async Task<List<DoctorDTO.DoctorBasic>> SearchDoctors(string keyword)
        {
            var doctors = await _context.Doctors.Include(d => d.User).Where(d => d.User.FullName.Contains(keyword)).ToListAsync();

            var doctorDTOs = _mapper.Map<List<DoctorDTO.DoctorBasic>>(doctors);

            return doctorDTOs;
        }

        public async Task<DoctorDTO.DoctorDetail> GetDoctorById(int doctorId)
        {
            var doctor = await _context.Doctors.Include(p => p.User).FirstOrDefaultAsync(d => d.UserId == doctorId);

            var doctorDTO = _mapper.Map<DoctorDTO.DoctorDetail>(doctor);

            return doctorDTO;
        }

        public async Task<List<DoctorDTO.DoctorSalaryDTO>> GetDoctorSalariesAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);

            var bonusMap = await CalculateTopDoctorBonusesAsync(month);

            var doctorAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Include(a => a.Doctor.Specialty)
                .Include(a => a.Service)
                .Where(a => a.AppointmentDate >= startDate &&
                    a.AppointmentDate < endDate &&
                    (a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))

                .ToListAsync();

            var doctorGroups = doctorAppointments
                .Where(a => a.DoctorId.HasValue)
                .GroupBy(a => a.DoctorId.Value);

            var result = new List<DoctorDTO.DoctorSalaryDTO>();

            foreach (var group in doctorGroups)
            {
                var firstAppointment = group.First();
                var doctor = firstAppointment.Doctor;
                var baseSalary = 4000000;
                var commissionTotal = group.Sum(a => (decimal)(a.Service?.Price ?? 0) * 0.3m);
                var bonus = bonusMap.TryGetValue(doctor.DoctorId, out var b) ? b : 0;

                result.Add(new DoctorDTO.DoctorSalaryDTO
                {
                    DoctorId = doctor.DoctorId,
                    DoctorName = doctor.User.FullName,
                    Specialty = doctor.Specialty.Name,
                    BaseSalary = baseSalary,
                    Commission = commissionTotal,
                    Bonus = bonus
                    // Không cần set TotalSalary vì là property tính toán
                });
            }

            return result;
        }

        public async Task<SalarySummaryDTO> GetSalarySummaryAsync(DateTime month)
        {
            Console.WriteLine("Tháng : ",month);
            var doctorSalaries = await GetDoctorSalariesAsync(month);

            var totalCommission = doctorSalaries.Sum(d => d.Commission);
            var totalSalary = doctorSalaries.Sum(d => d.TotalSalary);
            var grossRevenue = totalCommission/30*100;
            var netRevenue = grossRevenue - totalSalary;

            return new SalarySummaryDTO
            {
                TotalCommission = totalCommission,
                TotalSalary = totalSalary,
                GrossRevenue = grossRevenue,
                NetRevenue = netRevenue,
                DoctorSalaries = doctorSalaries
            };
        }


        public async Task<PaginatedResult<DoctorDTO.DoctorBasic>> GetDoctorsPaged(int pageNumber, string specialty = null, string searchKeyword = null)
        {
            int pageSize = 12;
            
            // Bắt đầu với truy vấn cơ bản
            var query = _context.Doctors.Include(doctor => doctor.User).Include(doctor => doctor.Specialty).AsQueryable();
            
            // Áp dụng bộ lọc nếu có
            if (!string.IsNullOrEmpty(specialty) && specialty.ToLower() != "all")
            {
                query = query.Where(d => d.Specialty.Name == specialty);
            }
            
            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(d => d.User.FullName.Contains(searchKeyword) || d.Specialty.Name.Contains(searchKeyword) || d.Position.Contains(searchKeyword));
            }
            
            // Đếm tổng số bác sĩ thỏa mãn điều kiện
            var totalItems = await query.CountAsync();
            
            // Lấy dữ liệu theo trang
            var doctors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var doctorDTOs = _mapper.Map<List<DoctorDTO.DoctorBasic>>(doctors);
            
            // Trả về kết quả phân trang
            return new PaginatedResult<DoctorDTO.DoctorBasic>(doctorDTOs, totalItems, pageNumber, pageSize);
        }


       public async Task<DoctorDTO.DoctorSalaryDetailResultDTO> GetDoctorSalaryDetailsAsync(int doctorId, DateTime month)
{
    var startDate = new DateTime(month.Year, month.Month, 1);
    var endDate = startDate.AddMonths(1);

    var bonusMap = await CalculateTopDoctorBonusesAsync(month);
    var bonus = bonusMap.TryGetValue(doctorId, out var b) ? b : 0;

    var appointments = await _context.Appointments
        .Include(a => a.Service)
        .Include(a => a.Patient).ThenInclude(p => p.User)
        .Where(a => a.DoctorId == doctorId &&
                    a.AppointmentDate.HasValue &&
                    a.AppointmentDate.Value >= startDate &&
                    a.AppointmentDate.Value < endDate &&
                    (a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))
        .ToListAsync();

    var details = appointments.Select(a => new DoctorDTO.DoctorSalaryDetailDTO
    {
        PatientName = a.Patient?.User?.FullName ?? "Không rõ",
        ServiceName = a.Service?.ServiceName ?? "Không rõ",
        ServicePrice = (decimal)(a.Service?.Price ?? 0),
        AppointmentDate = a.AppointmentDate ?? DateTime.MinValue,
        Commission = (decimal)(a.Service?.Price ?? 0) * 0.3m
    }).ToList();

    decimal commissionTotal = (decimal)details.Sum(d => d.Commission);
    var baseSalary = 4000000;
    decimal totalSalary = baseSalary + (decimal)commissionTotal + bonus;

    // foreach (var d in details)
    // {
    //     commissionTotal += d.Commission;
    //     Console.WriteLine($"Hoa hồng: {d.Commission} cho dịch vụ {d.ServiceName}, bệnh nhân {d.PatientName}");
    //     Console.WriteLine($"Tổng hoa hồng: {commissionTotal}");
    // }

    return new DoctorDTO.DoctorSalaryDetailResultDTO
    {
        Details = details,
        BaseSalary = baseSalary,
        Bonus = bonus,
        CommissionTotal = commissionTotal,
        TotalSalary = (decimal)totalSalary
    };
}



        public async Task<Dictionary<int, decimal>> CalculateTopDoctorBonusesAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);

            var topData = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialty)
                .Where(a => a.AppointmentDate >= startDate &&
                    a.AppointmentDate < endDate &&
                    (a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))
                .GroupBy(a => new
                {
                    a.DoctorId,
                    a.Doctor.Specialty.Name
                })
                .Select(g => new
                {
                    DoctorId = g.Key.DoctorId.Value,
                    Specialty = g.Key.Name,
                    AppointmentCount = g.Count()
                })
                .ToListAsync();

            var bonuses = new Dictionary<int, decimal>();
            var groupedBySpecialty = topData.GroupBy(x => x.Specialty);
            foreach (var group in groupedBySpecialty)
            {
                var topDoctors = group.OrderByDescending(x => x.AppointmentCount).Take(3).ToList();
                if (topDoctors.Count > 0) bonuses[topDoctors[0].DoctorId] = 3000000;
                if (topDoctors.Count > 1) bonuses[topDoctors[1].DoctorId] = 2000000;
                if (topDoctors.Count > 2) bonuses[topDoctors[2].DoctorId] = 1000000;
            }

            return bonuses;
        }


    }
}