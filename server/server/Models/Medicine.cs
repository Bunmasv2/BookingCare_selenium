using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string? MedicalName { get; set; }

    public int? StockQuantity { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string? Unit { get; set; }
    public float? Price { get; set; }
}
