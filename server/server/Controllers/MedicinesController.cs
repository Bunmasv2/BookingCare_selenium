using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
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
using Server.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Clinic_Management.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly ClinicManagementContext _context;
        private readonly IMedicine _medicineService;

        public MedicinesController(ClinicManagementContext context, IMedicine medicineService)
        {
            _context = context;
            _medicineService = medicineService;
        }

        // GET: api/medicines
        [HttpGet]
        public async Task<List<MedicineDTO.MedicineBasic>> GetAllMedicines()
        {
            return await _medicineService.GetAllMedicines();
        }
                // GET: api/medicines/search?query=para
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MedicineDTO.MedicineBasic>>> SearchMedicines([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return await _medicineService.GetAllMedicines();
            }

            var medicines = await _medicineService.SearchMedicinesByName(query);
            return Ok(medicines);
        }
    }
}