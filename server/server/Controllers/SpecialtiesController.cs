using Microsoft.AspNetCore.Authorization;
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
    public class SpecialtiesController : ControllerBase
    {
        private readonly ClinicManagementContext _context;
        private readonly ISpecialty _speciatyService;

        public SpecialtiesController(ClinicManagementContext context, ISpecialty speciatyService)
        {
            _context = context;
            _speciatyService = speciatyService;
        }

        // GET: api/Specialties
        [HttpGet]
        public async Task<List<SpecialtyDTO>> GetSpecialties()
        {
            return await _speciatyService.GetSpecialties();
        }

        // GET: api/Specialties/specialty/description
        [HttpGet("{specialty}/description")]
        public async Task<ActionResult<SpecialtyDTO>> GetDescription(string specialty)
        {
            if (string.IsNullOrEmpty(specialty))
            {
                throw new ErrorHandlingException(500, "UserName is required");
            }

            SpecialtyDTO specialtyDTO = await _speciatyService.GetDescription(specialty);

            return Ok(specialtyDTO);
        }

        [HttpGet("random")]
        public async Task<ActionResult> GetRandomSpecialties()
        {
            var specialties = await _speciatyService.GetRandomSpecialties();

            if (specialties.Count() == 0 || specialties == null) throw new ErrorHandlingException("Lỗi không lấy được chuyên khoa!");

            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialty(int id)
        {
            var specialty = await _speciatyService.GetById(id);
            if (specialty == null)
                return NotFound();
            return Ok(specialty);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSpecialty(Specialty specialty)
        {
            var result = await _speciatyService.Create(specialty);
            return CreatedAtAction(nameof(GetSpecialty), new { id = result.SpecialtyId }, result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialty(int id, Specialty specialty)
        {
            var success = await _speciatyService.Update(id, specialty);
            if (!success)
                return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialty(int id)
        {
            var success = await _speciatyService.Delete(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromForm] int specialtyId)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                Specialty specialty = await _context.Specialties.FindAsync(specialtyId);
                if (specialty == null)
                {
                    return NotFound("Service not found.");
                }

                specialty.SpecialtyImage = imageData;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded successfully." });
            }
            catch (ErrorHandlingException ex)
            {
                if (ex is ErrorHandlingException) throw;

                throw new ErrorHandlingException(500, ex.Message);
            }
        }
    }
}
