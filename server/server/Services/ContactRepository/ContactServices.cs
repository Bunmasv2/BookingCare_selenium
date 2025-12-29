using Microsoft.EntityFrameworkCore;
using server.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using server.DTO;
using server.Middleware;
using server.Util;

namespace server.Services
{
    public class ContactServices : IContact
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;
        public ContactServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ContactMessages> SendMessage(ContactMessages contactMessages)
        {
            var contact = await _context.AddAsync(contactMessages);
            await _context.SaveChangesAsync();
            return contact.Entity;
        }

        public async Task<List<ContactMessagesDTO.ContactMessages>> GetContactMessages()
        {
            List<ContactMessages> contactMessages = await _context.ContactMessages.Include(c => c.Patient).Include(c => c.Patient.User).ToListAsync();

            var contactMessageDTOs = _mapper.Map<List<ContactMessagesDTO.ContactMessages>>(contactMessages);

            return contactMessageDTOs;
        }

        public async Task<bool> ReponseEmail(IConfiguration _configuration, ContactMessages contactMessages, string message)
        {
            try
            {
                Console.WriteLine("Đang ở ReponseEMil: ");
                string Email = contactMessages.Patient.User.Email ?? throw new ErrorHandlingException(400, "Không tìm thấy email của khách hàng!!");
                string subject = $"Trả lời liên hệ của anh/chị {contactMessages.Patient.User.FullName}";
                string body = $@"<p>Họ tên:<b> {contactMessages.Patient.User.FullName}</b></p>
                            <p>Nội dung của anh/chị:<b> {contactMessages.Messages}</b></p>
                            <p>Gửi lúc:<b> {contactMessages.CreatedAt}</b></p>";

                body += $@"<br><br>
                        <p><b>Nội dung phản hồi:</b> {message}</p>";
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxx: " + Email + "IIIIIIIIIIII: " + subject);
                Console.WriteLine("ZZZZZZZZZZZZZZZZZZ: " + body);
                await EmailUtil.SendEmailAsync(_configuration, Email, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
                return false;
            }
        }
    }
   
}