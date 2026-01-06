using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Services;
using server.Util;
using Server.DTO;
using System.Globalization;

namespace server.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly ClinicManagementContext _context;
        private readonly IDoctor _doctorService;
        private readonly IPatient _patientService;
        private readonly IAppointment _appointmentService;
        private readonly IService _serviceServices;
        private readonly ISpecialty _specialtyService;
        private readonly IConfiguration _configuration;

        public AppointmentsController(ClinicManagementContext context, IDoctor doctorService, IPatient patientService, IAppointment appointmentService, ISpecialty specialtyService, IService serviceServices, IConfiguration configuration)
        {
            _context = context;
            _doctorService = doctorService;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _specialtyService = specialtyService;
            _serviceServices = serviceServices;
            _configuration = configuration;
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public async Task<ActionResult> Appointment([FromForm] AppointmentForm appointmentForm)
        {
            if (appointmentForm == null)
                throw new ErrorHandlingException(400, "Dữ liệu không hợp lệ");

            if (string.IsNullOrWhiteSpace(appointmentForm.Department))
                throw new ErrorHandlingException(400, "Vui lòng chọn khoa");

            if (string.IsNullOrWhiteSpace(appointmentForm.Doctor))
                throw new ErrorHandlingException(400, "Vui lòng chọn bác sĩ");

            if (string.IsNullOrWhiteSpace(appointmentForm.Service))
                throw new ErrorHandlingException(400, "Vui lòng chọn dịch vụ");

            if (string.IsNullOrWhiteSpace(appointmentForm.AppointmentDate))
                throw new ErrorHandlingException(400, "Vui lòng chọn ngày khám");

            if (string.IsNullOrWhiteSpace(appointmentForm.AppointmentTime))
                throw new ErrorHandlingException(400, "Vui lòng chọn buổi khám");

            // if (!string.IsNullOrWhiteSpace(appointmentForm.Symptoms) && appointmentForm.Symptoms.Count() > 500)
            //     throw new ErrorHandlingException(400, "Triệu chứng quá dài");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var department = await _specialtyService.GetSpecialty(appointmentForm.Department)
                ?? throw new ErrorHandlingException(404, "Không tìm thấy khoa");

            var doctor = await _doctorService.GetDoctorByName(appointmentForm.Doctor)
                ?? throw new ErrorHandlingException(404, "Không tìm thấy bác sĩ");

            var patient = await _patientService.GetPatientByUserId(parsedUserId)
                ?? throw new ErrorHandlingException(404, "Không tìm thấy bệnh nhân");

            var service = await _serviceServices.GetServiceByName(appointmentForm.Service)
                ?? throw new ErrorHandlingException(404, "Không tìm thấy dịch vụ");

            var appointmentDate = DateOnly.Parse(appointmentForm.AppointmentDate).ToDateTime(TimeOnly.MinValue);

            TimeZoneInfo vnTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime today =
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;

            int diffDays = (appointmentDate - today).Days;

            if (diffDays < 1)
                throw new ErrorHandlingException(
                    400,
                    "Vui lòng đặt lịch khám tối thiểu trước 1 ngày"
                );

            if (diffDays > 15)
                throw new ErrorHandlingException(
                    400,
                    "Ngày khám không được cách quá 15 ngày so với hôm nay"
                );

            var isExistAppointment = await _appointmentService.IsExistAppointment(
                patient.PatientId,
                appointmentDate.Date,
                appointmentForm.AppointmentTime
            );

            if (isExistAppointment != null)
            {
                var dateOnly =
                    DateOnly.FromDateTime(isExistAppointment.AppointmentDate!.Value);

                throw new ErrorHandlingException(
                    400,
                    $"Bạn chưa hoàn thành lịch hẹn {dateOnly} {isExistAppointment.AppointmentTime}"
                );
            }

            int quantityAppointment =
                await _appointmentService.CountAppointsByDate(
                    appointmentDate.Date,
                    appointmentForm.AppointmentTime
                );

            if (quantityAppointment > 15)
                throw new ErrorHandlingException(
                    400, 
                    "Bác sĩ đã đủ số lượng bệnh nhân trong ngày này, vui lòng chọn ngày hoặc khung giờ khác"
                );

            await _appointmentService.Appointment(
                patient.PatientId,
                doctor.DoctorId,
                service.ServiceId,
                appointmentDate.Date,
                appointmentForm.AppointmentTime,
                "Chờ xác nhận"
            );

            return Ok(new { message = "Đặt lịch thành công!" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<ActionResult<List<AppointmentDTO.AppointmentDetail>>> GetAppointments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var appointments = await _appointmentService.GetAppointments();

            return Ok(appointments);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{month}/{year}")]
        public async Task<ActionResult<List<AppointmentDTO.AppointmentDetail>>> GetAppointmentsByMonthYear(int month, int year)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var appointments = await _appointmentService.GetAppointmentsByMonthYear(month, year);

            return Ok(appointments);
        }

        [Authorize(Roles = "admin, doctor")]
        [HttpPut("status/{id}")]
        public async Task<ActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDTO statusUpdate)
        {
            var role = HttpContext.Items["role"].ToString();

            if (role == "doctor" && statusUpdate.Status != "Đã khám")
            {
                throw new ErrorHandlingException(403, "Bạn không có quyền!");
            }

            var appointment = await _appointmentService.GetAppointmentById(id) ?? throw new ErrorHandlingException(404, "không tìm thấy lịch hẹn");
            string oldStatus = appointment.Status;

            await _appointmentService.UpdateStatus(appointment, statusUpdate.Status);

            if (statusUpdate.Status != "Đã xác nhận")
            {
                return Ok(new { message = "Cập nhật trạng thái thành công" });
            }

            await SendStatusUpdateEmail(
                appointment.Patient.User.Email,
                appointment.Patient.User.FullName,
                appointment.Doctor?.User?.FullName,
                appointment.AppointmentDate.Value,
                appointment.Service?.ServiceName,
                oldStatus,
                statusUpdate.Status
            );

            return Ok(new { message = "Xác nhận lịch hẹn thành công" });
        }

        private async Task<bool> SendStatusUpdateEmail(string email, string patientName, string doctorName, DateTime appointmentDate, string serviceName, string oldStatus, string newStatus)
        {
            try
            {
                string formattedDate = appointmentDate.ToString("dd/MM/yyyy");

                var Subject = "Thông báo cập nhật trạng thái lịch hẹn";

                var Body = $@"
                        <html>
                        <body>
                            <h2>Cập nhật trạng thái lịch hẹn</h2>
                            <p>Xin chào {patientName},</p>
                            <p>Lịch hẹn khám bệnh của bạn đã được cập nhật trạng thái.</p>
                            <p><strong>Thông tin lịch hẹn:</strong></p>
                            <ul>
                                <li>Bác sĩ: {doctorName}</li>
                                <li>Dịch vụ: {serviceName}</li>
                                <li>Ngày hẹn: {formattedDate}</li>
                                <li>Trạng thái cũ: {oldStatus}</li>
                                <li>Trạng thái mới: <strong>{newStatus}</strong></li>
                            </ul>
                            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                            <p>Trân trọng,<br/>Hệ thống đặt lịch khám bệnh</p>
                        </body>
                        </html>";

                await EmailUtil.SendEmailAsync(_configuration, email, Subject, Body);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        [Authorize(Roles = "patient")]
        [HttpPut("cancel/{appointmentId}")]
        public async Task<ActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new ErrorHandlingException("Không tìm thấy lịch hẹn");
            if (appointment.Status == "Đã khám" || appointment.Status == "Đã hoàn thành" || appointment.Status == "Đã thanh toán")
            {
                throw new ErrorHandlingException($"Không thể hủy cuộc hẹn {appointment.Status}");
            }
            appointment.Status = "Đã hủy";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        // [Authorize(Roles = "patient")]
        // [HttpPost("by-patient/{quantity}")]
        // public async Task<ActionResult> GetAppointmentByPatientId(int quantity)
        // {
        //     var userId = HttpContext.Items["UserId"];
        //     int parsedUserId = Convert.ToInt32(userId.ToString());

        //     Console.WriteLine("UserId: " + parsedUserId);

        //     var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException("Không tim thấy bệnh nhân");

        //     var appointments = await _appointmentService.GetAppointmentByPatientId(patient.PatientId, quantity);
        //     return Ok(appointments);
        // }
        [Authorize(Roles = "patient")]
        [HttpPost("by-patient")]
        public async Task<ActionResult> GetAppointmentByPatientId([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException("Không tim thấy bệnh nhân");
            if (!patient.PatientId.HasValue)
                throw new ErrorHandlingException("Không tìm thấy ID của bệnh nhân");

            var totalAppointments = await _appointmentService.CountAppointmentsByPatientId(patient.PatientId.Value);

            int totalPages = (int)Math.Ceiling((double)totalAppointments / pageSize);

            // Đảm bảo page không nhỏ hơn 1 và không lớn hơn totalPages
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var appointments = await _appointmentService.GetAppointmentsByPatientIdPaginated(patient.PatientId.Value, page, pageSize);

            return Ok(new
            {
                appointments,
                currentPage = page,
                pageSize,
                totalPages,
                totalItems = totalAppointments
            });
        }

        [Authorize(Roles = "doctor")]
        [HttpGet("schedule")]
        public async Task<ActionResult<List<AppointmentDTO.DoctorScheduleDTO>>> GetDoctorSchedule()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var doctor = await _doctorService.GetDoctorById(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bác sĩ!");

            var schedule = await _appointmentService.GetDoctorSchedule(doctor.DoctorId) ?? throw new ErrorHandlingException("Không tìm thấy lịch làm việc!");

            return schedule;
        }

        [Authorize(Roles = "doctor")]
        [HttpGet("schedule_detail")]
        public async Task<ActionResult> GetDoctorScheduleByDateTime([FromQuery] string date, [FromQuery] string time)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var doctor = await _doctorService.GetDoctorById(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bác sĩ!");

            var schedules = await _appointmentService.GetDoctorScheduleDetail(doctor.DoctorId, date, time);

            return Ok(new { schedules = schedules, doctor = doctor });
        }

        [Authorize(Roles = "doctor")]
        [HttpGet("examined_patients")]
        public async Task<ActionResult> GetPatientByStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var doctor = await _doctorService.GetDoctorById(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bác sĩ!");

            var schedules = await _appointmentService.GetPatientScheduleDetail(doctor.DoctorId);

            return Ok(new { schedules = schedules });
        }

        [Authorize(Roles = "patient")]
        [HttpGet("recently")]
        public async Task<ActionResult> GetRecentAppointment()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = Convert.ToInt32(userId);

            var patient = await _patientService.GetPatientByUserId(parsedUserId) ?? throw new ErrorHandlingException("Không tìm thấy bệnh nhân!");

            var appointment = await _appointmentService.GetRecentAppointment(patient.PatientId);

            return Ok(appointment);
        }

        [HttpGet("examined/{doctorId}")]
        public async Task<ActionResult> GetExaminedPatientCount(int doctorId)
        {
            var examinedPatientCount = await _appointmentService.GetExaminedPatientCount(doctorId);

            return Ok(examinedPatientCount);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("statistics/{month}/{year}")]
        public async Task<ActionResult> AppointmentStatistics(int month, int year)
        {
            var appointment = await _appointmentService.AppointmentStatistics(month, year);

            return Ok(appointment);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("statistics/{month}/week")]
        public async Task<ActionResult> AppointmentStatisticsPerWeek(int month)
        {
            var appointment = await _appointmentService.AppointmentStatisticsPerWeek(month);

            return Ok(appointment);
        }
    }
}