using Microsoft.AspNetCore.Mvc;
using server.DTO;
using server.Models;

namespace server.Services
{
    public interface IContact
    {
        Task<ContactMessages> SendMessage(ContactMessages contactMessages);
        Task<List<ContactMessagesDTO.ContactMessages>> GetContactMessages();
        Task<bool> ReponseEmail(IConfiguration _configuration, ContactMessages contactMessages, string message);
    }
}
