using System;
using System.Collections.Generic;

namespace server.Models;

public partial class ContactMessages
{
    public int Id { get; set; }
    public int? PatientId { get; set; }
    public string? Messages  { get; set; }
    public string Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual Patient? Patient { get; set; }
}