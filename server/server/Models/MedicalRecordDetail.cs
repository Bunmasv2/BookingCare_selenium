using System;
using System.Collections.Generic;

namespace server.Models;

public partial class MedicalRecordDetail
{
    public int? MedicineId { get; set; }

    public int? ReCordId { get; set; }

    public int? Quantity { get; set; }

    public int? Dosage { get; set; }

    public int? FrequencyPerDay { get; set; }

    public int? DurationInDays { get; set; }

    public string? Usage { get; set; }

    public virtual Medicine? Medicine { get; set; }

    public virtual MedicalRecord? ReCord { get; set; }
}
