namespace server.DTO
{
    public class ReviewForm
    {
        public int RecordId { get; set; }
        public int OverallRating { get; set; }
        public string? Comment { get; set; }
        public DoctorRatings? DoctorRatings { get; set; }
        public ServiceRatings? ServiceRatings { get; set;  }
    }

    public class DoctorRatings
    {
        public int Knowledge { get; set; }
        public int Attitude { get; set; }
        public int Dedication { get; set; }
        public int CommunicationSkill { get; set; }
    }

    public class ServiceRatings
    {
        public int Effectiveness { get; set; }
        public int Price { get; set; }
        public int ServiceSpeed { get; set; }
        public int Convenience { get; set; }
    }
}
