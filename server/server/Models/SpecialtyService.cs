using System;
using System.Collections.Generic;

namespace server.Models;

public partial class SpecialtyService
{
    public int SpecialtyId { get; set; }

    public int ServiceId { get; set; }

    public Specialty Specialty { get; set; }

    public Service Service { get; set; }
}
