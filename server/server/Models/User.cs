using System;
using System.Collections.Generic;

namespace server.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
}
