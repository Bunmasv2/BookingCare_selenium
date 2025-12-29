using System;
using System.Collections.Generic;

namespace server.Models;

public class Review
{
    public int ReviewId { get; set; }
    public int PrescriptionId { get; set; }
    public int OverallRating { get; set; } 
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual MedicalRecord? MedicalRecord { get; set; }
    public DoctorReviewDetail? DoctorReviewDetail { get; set; }
    public ServiceReviewDetail? ServiceReviewDetail { get; set; }
}