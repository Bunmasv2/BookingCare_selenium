using System;
using System.Collections.Generic;

namespace server.Models;

public class DoctorReviewDetail
{
    public int DoctorReviewDetailId { get; set; }

    public int ReviewId { get; set; }
    public int Knowledge { get; set; }
    public int Attitude { get; set; }
    public int Dedication { get; set; }
    public int CommunicationSkill { get; set; }
    public Review Review { get; set; }
}