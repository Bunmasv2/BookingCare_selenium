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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatient _patientService;

        public PatientsController(IPatient patientService)
        {
            _patientService = patientService;
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<ActionResult<List<PatientDTO.PatientDetail>>> GetPatients()
        {
            var userId = HttpContext.Items["UserId"];
            int parsedUserId = Convert.ToInt32(userId.ToString());

            var patients = await _patientService.GetAllPatients();

            return Ok(patients);
        }


        // GET: api/Patients/user
        // [HttpGet("user")]
        // public async Task<ActionResult<PatientDTO.PatientDetail>> GetPatientByUserId()
        // {
        //    var userId = HttpContext.Items["UserId"];
        //    var parseUserId = Convert.ToInt32(userId.ToString());
            
        //    var patient = await _patientService.GetPatientByUserId(parseUserId);

        //    if (patient == null)
        //    {
        //        throw new ErrorHandlingException("Patient not found");
        //    }

        //    return Ok(patient);
        // }
    }
}