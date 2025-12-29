using System;
using System.Collections.Generic;

namespace server.Models;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public int? AppointmentId { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    // public float? Price {get; set;}
    public virtual Appointment? Appointment { get; set; }
    public virtual Review? Review { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<MedicalRecordDetail> MedicalRecordDetails { get; set; } = new List<MedicalRecordDetail>();
}