using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdmin _adminService;

        public AdminsController(IAdmin adminService)
        {
            _adminService = adminService;
        }

        // GET: api/Patients/search?keyword=abc
        //[HttpGet("search")]
        //public async Task<ActionResult<List<PatientDTO.PatientBasic>>> SearchPatients([FromQuery] string keyword)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //        return BadRequest("Keyword is required");

        //    var patients = await _patientService.SearchPatients(keyword);

        //    return Ok(patients);
        //}

        // POST: api/Patients
        //[Authorize(Roles = "admin")]
        //[HttpPost]
        //public async Task<ActionResult<PatientDTO.PatientDetail>> CreatePatient(PatientDTO.PatientDetail patientDTO)
        //{
        //    try
        //    {
        //        var createdPatient = await _patientService.CreatePatient(patientDTO);
        //        return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.PatientId }, createdPatient);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ErrorHandlingException(500, ex.Message);
        //    }
        //}

        // PUT: api/Patients/5
        //[Authorize(Roles = "admin,patient")]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePatient(int id, PatientDTO.PatientDetail patientDTO)
        //{
        //    try
        //    {
        //        var updatedPatient = await _patientService.UpdatePatient(id, patientDTO);
                
        //        if (updatedPatient == null)
        //        {
        //            return NotFound();
        //        }
                
        //        return Ok(updatedPatient);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ErrorHandlingException(500, ex.Message);
        //    }
        //}

        // DELETE: api/Patients/5
        //[Authorize(Roles = "admin")]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePatient(int id)
        //{
        //    try
        //    {
        //        var result = await _patientService.DeletePatient(id);
                
        //        if (!result)
        //        {
        //            return NotFound();
        //        }
                
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ErrorHandlingException(500, ex.Message);
        //    }
        //}
    }
}