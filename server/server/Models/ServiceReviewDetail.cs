using System;
using System.Collections.Generic;

namespace server.Models;

public class ServiceReviewDetail
{
    public int ServiceReviewDetailId { get; set; }
    public int ReviewId { get; set; }
    public int Effectiveness { get; set; }
    public int Price { get; set; }
    public int ServiceSpeed { get; set; }
    public int Convenience { get; set; }
    public Review Review { get; set; }
}
