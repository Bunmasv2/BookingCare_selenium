using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace server.Models;

public partial class LoginForm
{
    [Required(ErrorMessage = "Email không được để trống!")]
    [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng!")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password không được để trống!")]
    [MinLength(6, ErrorMessage = "Password tối thiểu 6 ký tự!")]
    public string? Password { get; set; }
}
