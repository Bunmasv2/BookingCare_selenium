using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public float? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public byte[]? ServiceImage { get; set; }
    public byte[]? ServiceIcon { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ServiceRegistration> ServiceRegistrations { get; set; } = new List<ServiceRegistration>();

    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}
