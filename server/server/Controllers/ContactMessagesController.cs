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
    // [Authorize(Roles = "patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessagesController : ControllerBase
    {
        private readonly IContact _contactService;
        private readonly IPatient _patientServices;
        private readonly IConfiguration _configuration;
        private readonly ClinicManagementContext _context;
        public ContactMessagesController(ClinicManagementContext context, IContact contactService, IPatient patientServices, IConfiguration configuration)
        {
            _context = context;
            _contactService = contactService;
            _patientServices = patientServices;
            _configuration = configuration;
        }

        [Authorize(Roles = "patient")]
        [HttpPost("{message}")]
        public async Task<ActionResult> SendMessage(string message)
        {
            var userId = HttpContext.Items["UserId"];
            var parseUserId = Convert.ToInt32(userId.ToString());

            var patient = await _patientServices.GetPatientByUserId(parseUserId) ?? throw new ErrorHandlingException(404, "Patient not found");

            if (message == "") throw new ErrorHandlingException(400, "Vui lòng nhập nội dung!");

            ContactMessages contactMessages = new ContactMessages
            {
                PatientId = patient.PatientId,
                Messages = message,
                CreatedAt = DateTime.Now,
                Status = "Chưa phản hồi"
            };

            ContactMessages contact = await _contactService.SendMessage(contactMessages) ?? throw new ErrorHandlingException("Gửi thất bại!");

            return Ok(new { message = "Gửi thành công" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult> GetAllContactMessages()
        {
            List<ContactMessagesDTO.ContactMessages> contactMessages = await _contactService.GetContactMessages();
            return Ok(contactMessages);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("reponse-message/{id}")]
        public async Task<ActionResult> ReponseMessage(int id, [FromBody] ContactMessagesDTO.ReplyDto message)
        {
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB: " + message.Message);
           ContactMessages contactMessages = await _context.ContactMessages
                                            .Include(c => c.Patient)
                                            .ThenInclude(p => p.User)
                                            .FirstOrDefaultAsync(c => c.Id == id) 
                                            ?? throw new ErrorHandlingException(400, "Không tìm thấy câu hỏi của khách hàng");

            Console.WriteLine("CCCCCCCCCCCCCCCCCCCCCC: " + contactMessages.Messages);
            bool check = await _contactService.ReponseEmail(_configuration, contactMessages, message.Message);
            Console.WriteLine("dddddDDDDDDDDDDDDDDDDDDD: " + check.ToString());
            if (check)
            {
                contactMessages.Status = "Đã phản hồi";
                await _context.SaveChangesAsync();
                return Ok("Đã gửi câu phản hồi");
            }
            else
            {
                return BadRequest("Câu phản hồi chưa được gửi!");
            }
        }
    }
}