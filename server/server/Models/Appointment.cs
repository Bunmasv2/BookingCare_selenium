using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? Status { get; set; }

    public int? ServiceId { get; set; }

    public string? AppointmentTime { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Service? Service { get; set; }

    public virtual ICollection<ServiceRegistration> ServiceRegistrations { get; set; } = new List<ServiceRegistration>();
}
