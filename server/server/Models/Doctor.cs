using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int? UserId { get; set; }

    public int? SpecialtyId { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Position { get; set; }

    public string? Biography { get; set; }

    public string? Qualifications { get; set; }

    public string? WorkExperience { get; set; }

    public string? Degree { get; set; }

    public byte[]? DoctorImage { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Specialty? Specialty { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
