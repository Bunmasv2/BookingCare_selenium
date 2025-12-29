using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace server.Models
{
    public class SigninForm
    {
        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng!")]
        public string? Email { get; set; }
        

        [Required(ErrorMessage = "Password không được để trống!")]
        public string? Password { get; set; }
    }
}
