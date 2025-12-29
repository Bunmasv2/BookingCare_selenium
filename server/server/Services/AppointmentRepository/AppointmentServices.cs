using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Util;

namespace server.Services
{
    public class AppointmentServices : IAppointment
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public AppointmentServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Appointment> IsExistAppointment(int? patientId, DateTime appointmentDate, string appointmentTime)
        {
            var appointment = await _context.Appointments
                .Where(a => a.PatientId == patientId  && a.Status != "Đã khám" && a.Status != "Đã hoàn thành")
                .FirstOrDefaultAsync();

            return appointment;
        }

        public async Task<Appointment> Appointment(int? patientId, int? doctorId, int? serviceId, DateTime appointmentDate, string appointmentTime, string status)
        {
            Appointment appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ServiceId = serviceId,
                Status = status,
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<AppointmentDTO.AppointmentDetail>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        public async Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentsByMonthYear(int month, int year)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .Where(a => a.AppointmentDate.Value.Month == month && a.AppointmentDate.Value.Year == year)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        public async Task UpdateStatus(Appointment appointment, string newStatus)
        {
            appointment.Status = newStatus;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentByPatientId(int? patientId, int quantity)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .Take(quantity)
                .ToListAsync();

            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        // public async Task<Appointment> GetAppointmentById(int appointmentId)
        // {
        //     return await _context.Appointments.FindAsync(appointmentId);
        // }

        public void CancelAppointment(Appointment appointment)
        {
            appointment.Status = "Đã hủy";
            _context.SaveChangesAsync();
        }

        public async Task<List<AppointmentDTO.DoctorScheduleDTO>> GetDoctorSchedule(int? doctorId)
        {
            var schedule = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && (a.Status == "Đã xác nhận" || a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))
                .ToListAsync();

            var groupedSchedule = schedule
                .GroupBy(a => new
                {
                    Date = a.AppointmentDate.Value.Date,
                    AppointmentTime = a.AppointmentTime
                })
                .Select(g => new AppointmentDTO.DoctorScheduleDTO
                {
                    Date = g.Key.Date,
                    AppointmentTime = g.Key.AppointmentTime,
                    PatientCount = g.Count()
                })
                .OrderBy(g => g.Date)
                .ThenBy(g => g.AppointmentTime)
                .ToList();

            return groupedSchedule;
        }

        public async Task<List<AppointmentDTO.AppointmentDetail>> GetDoctorScheduleDetail(int? doctorId, string date, string time)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .Where(a => a.DoctorId == doctorId && a.AppointmentTime == time && a.AppointmentDate == DateTime.Parse(date) && (a.Status == "Đã xác nhận" || a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))

                .ToListAsync();

            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        public async Task<List<AppointmentDTO.AppointmentDetail>> GetPatientScheduleDetail(int? doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .Where(a => a.DoctorId == doctorId && a.Status == "Đã khám")

                .ToListAsync();

            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        public async Task<List<int>> GetAppointmentsId(int? patientId)
        {
            var appointmentIds = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Select(a => a.AppointmentId)
                .ToListAsync() ?? throw new ErrorHandlingException("Lỗi khi lấy đanh sách lịch hẹn!");

            return appointmentIds;
        }

        public async Task<List<int>> GetRecentAppointmentsId(int? patientId)
        {
            var appointmentIds = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(3)
                .Select(a => a.AppointmentId)
                .ToListAsync() ?? throw new ErrorHandlingException("Lỗi khi lấy đanh sách lịch hẹn!");

            return appointmentIds;
        }

        public async Task<AppointmentDTO.AppointmentDetail> GetRecentAppointment(int? patientId)
        {
            var appointment = await _context.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDate >= DateTime.Now.Date)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentDate)
                .FirstOrDefaultAsync();

            var appointmentDTO = _mapper.Map<AppointmentDTO.AppointmentDetail>(appointment);

            return appointmentDTO;
        }

        public async Task<int> GetExaminedPatientCount(int doctorId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && (a.Status == "Đã khám" || a.Status == "Đã hoàn thành"))
                .CountAsync();

            return appointments;
        }

        public async Task<object> AppointmentStatistics(int month, int year)
        {
            var grouped = await _context.Appointments
                .Where(appointment => appointment.AppointmentDate.Value.Month == month && appointment.AppointmentDate.Value.Year == year)
                .GroupBy(appointment => appointment.Status)
                .Select(g => new AppointmentDTO.AppointmentGroup
                {
                    Status = g.Key,
                    Appointments = g.Count()
                })
                .ToListAsync();

            var statusOrder = new List<string>
            {
                "Chờ xác nhận",
                "Đã xác nhận",
                "Đã khám",
                // "Đã thanh toán",
                "Đã hoàn thành",
                "Đã hủy"
            };

            var ordered = statusOrder
                .Select(status => grouped.FirstOrDefault(g => g.Status == status)
                                ?? new AppointmentDTO.AppointmentGroup { Status = status, Appointments = 0 })
                .ToList();

            return ordered;
        }

        public async Task<object> AppointmentStatisticsPerWeek(int month)
        {
            var appointments = await _context.Appointments
                .Where(appointment => appointment.AppointmentDate.HasValue &&
                                    appointment.AppointmentDate.Value.Month == month)
                .ToListAsync();

            var groupedByWeek = appointments
                .GroupBy(a => Others.GetWeekOfMonth(a.AppointmentDate.Value))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Week = $"Tuần {g.Key}",
                    Appointments = g.Count()
                })
                .ToList();

            return groupedByWeek;
        }

        // Method để đếm tổng số lịch hẹn của một bệnh nhân
        public async Task<int> CountAppointmentsByPatientId(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .CountAsync();
        }

        // Method để lấy lịch hẹn theo trang
        public async Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentsByPatientIdPaginated(int patientId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Service)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var appointmentDTOs = _mapper.Map<List<AppointmentDTO.AppointmentDetail>>(appointments);

            return appointmentDTOs;
        }

        public async Task<int> CountAppointsByDate(DateTime date, string time)
        {
            var appointments = await _context.Appointments.Where(a => DateOnly.FromDateTime(a.AppointmentDate.Value.Date) == DateOnly.FromDateTime(date) && a.AppointmentTime == time).ToListAsync();

            return appointments.Count();
        }

        public async Task<List<AppointmentDTO.AvailableAppointment>> CheckAvailableAppointment(int? doctorId, DateTime date, string time)
        {
            var endDate = date.AddDays(15);

            var appointments = await _context.Appointments
                .Where(a =>
                    a.AppointmentDate.HasValue &&
                    a.AppointmentDate.Value.Date >= date.Date &&
                    !(a.AppointmentDate.Value.Date == date.Date && a.AppointmentTime == time) &&
                    a.AppointmentDate.Value.Date <= endDate.Date &&
                    a.DoctorId == doctorId
                )
                .ToListAsync();

            var slotCounts = appointments
                .GroupBy(a => new { Date = a.AppointmentDate.Value.Date, Time = a.AppointmentTime })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    Time = g.Key.Time,
                    Count = g.Count()
                })
                .ToList();

            var availableSlots = slotCounts
                .Where(s => s.Count < 12 && !(s.Date == date.Date && s.Time == time))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.Time == "Sáng" ? 0 : 1)
                .Take(3)
                .Select(s => new AppointmentDTO.AvailableAppointment
                {
                    Date = s.Date,
                    Time = s.Time
                })
                .ToList();

            return availableSlots;
        }

        public async Task<Appointment> GetAppointmentById(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Service)
                .Include(a => a.MedicalRecord)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            return appointment;
        }

        public async Task<int> CountAppointment(DateTime date, string time)
        {
            return 0;
        }
    }
}
