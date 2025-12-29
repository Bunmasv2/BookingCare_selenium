using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ClinicManagementContext _context;
        private readonly IDoctor _doctorService;

        public DoctorsController(ClinicManagementContext context, IDoctor doctorService)
        {
            _context = context;
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorDTO.DoctorBasic>>> GetAllDoctors()
        {   
            return await _doctorService.GetAllDoctors();
        }

        // Thêm vào DoctorController.cs
        [HttpGet("paged")]
        public async Task<IActionResult> GetDoctorsPaged([FromQuery] int pageNumber = 1, [FromQuery] string specialty = null, [FromQuery] string keyword = null)
        {
            try
            {
                var result = await _doctorService.GetDoctorsPaged(pageNumber, specialty, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách bác sĩ: {ex.Message}");
            }
        }
        [HttpGet("detail/{doctorName}")]
        public async Task<ActionResult<DoctorDTO.DoctorDetail>> GetDoctorByName(string doctorName)
        {
            if (string.IsNullOrEmpty(doctorName))
            {
                throw new ErrorHandlingException(500, "UserName is required");
            }

            DoctorDTO.DoctorDetail doctor = await _doctorService.GetDoctorByName(doctorName);

            return Ok(doctor);
        }
        [Authorize(Roles = "doctor")]
        [HttpGet("user")]
        public async Task<ActionResult<DoctorDTO.DoctorDetail>> GetPatientByUserId()
        {
            var userId = HttpContext.Items["UserId"];
            var parseUserId = Convert.ToInt32(userId.ToString());

            var patient = await _doctorService.GetDoctorById(parseUserId);

            if (patient == null)
            {
                throw new ErrorHandlingException(404, "Doctor not found");
            }

            return Ok(patient);
        }


        // [HttpGet("{id}")]
        // public async Task<ActionResult<Doctor>> GetDoctor(int id)
        // {
        //     var doctor = await _context.Doctors.FindAsync(id);
        //     if (doctor == null)
        //     {
        //         return NotFound();
        //     }
        //     return doctor;
        // }

        [HttpGet("{specialty}")]
        public async Task<ActionResult<List<DoctorDTO.DoctorBasic>>> GetDoctorsBySpecialty(string specialty)
        {
            List<DoctorDTO.DoctorBasic> doctors = await _doctorService.GetDoctorsBySpecialty(specialty);

            return Ok(doctors);
        }

        /// <summary>
        /// Tìm kiếm bác sĩ theo từ khóa (tên)
        /// GET: api/doctors/search?keyword=abc
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<DoctorDTO.DoctorBasic>> SearchDoctors([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required");

            List<DoctorDTO.DoctorBasic> doctors = await _doctorService.SearchDoctors(keyword);

            return Ok(doctors);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromForm] int doctorId)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                Doctor doctor = await _context.Doctors.FindAsync(doctorId);
                if (doctor == null)
                {
                    return NotFound("Doctor not found.");
                }

                doctor.DoctorImage = imageData;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded successfully." });
            }
            catch (ErrorHandlingException ex)
            {
                if (ex is ErrorHandlingException) throw;

                throw new ErrorHandlingException(500, ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("salary/monthly")]
        public async Task<IActionResult> GetDoctorSalaries([FromQuery] DateTime? month = null)
        {
            // Kiểm tra và xử lý tháng nếu null, gán giá trị hiện tại
            month ??= DateTime.Now;

            // Lấy chỉ năm và tháng từ ngày, loại bỏ phần giờ, phút, giây
            var startDate = new DateTime(month.Value.Year, month.Value.Month, 1);
            var endDate = startDate.AddMonths(1);

            // In ra giá trị của month đã xử lý
            Console.WriteLine($"Month is: {month.Value.ToString("yyyy-MM")}");  // In ra năm và tháng

            var salaries = await _doctorService.GetDoctorSalariesAsync(month.Value);
            return Ok(salaries);
        }

        [HttpGet("salary-summary")]
        public async Task<IActionResult> GetSalarySummary([FromQuery] DateTime? month)
        {
            var summary = await _doctorService.GetSalarySummaryAsync(month ?? DateTime.Now);
            return Ok(summary);
        }


        [Authorize(Roles = "admin")]
        [HttpGet("salary/details")]
        public async Task<IActionResult> GetDoctorSalaryDetails(int doctorId, DateTime? month)
        {
            
            // Kiểm tra và xử lý tháng nếu null, gán giá trị hiện tại
            month ??= DateTime.Now;

            // Lấy chỉ năm và tháng từ ngày, loại bỏ phần giờ, phút, giây
            var startDate = new DateTime(month.Value.Year, month.Value.Month, 1);
            var endDate = startDate.AddMonths(1);
            Console.WriteLine("Doctor Id: "+ doctorId +$"Month is: {month.Value.ToString("yyyy-MM")}" );
            var result = await _doctorService.GetDoctorSalaryDetailsAsync(doctorId, month.Value);
            return Ok(result);
        }

    }
}
