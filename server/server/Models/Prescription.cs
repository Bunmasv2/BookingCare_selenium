using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int? RecordId { get; set; }

    public string? Medicine { get; set; }

    public string? Dosage { get; set; }

    public virtual MedicalRecord? Record { get; set; }
}
