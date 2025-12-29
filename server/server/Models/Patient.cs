using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Patient
{
    public int PatientId { get; set; }
    public int? UserId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<ServiceRegistration> ServiceRegistrations { get; set; } = new List<ServiceRegistration>();
    public virtual ApplicationUser? User { get; set; }
    public virtual ContactMessages? ContactMessages { get; set; }
}
