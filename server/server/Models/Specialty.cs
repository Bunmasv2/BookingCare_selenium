using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Specialty
{
    public int SpecialtyId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    public byte[]? SpecialtyImage { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
