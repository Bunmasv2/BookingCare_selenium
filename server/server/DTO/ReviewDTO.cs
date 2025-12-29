namespace server.DTO
{
    public class ReviewDTO
    {
        public int RecordId { get; set; }
        public int OverallRating { get; set; }
        public string? Comment { get; set; }
        public int Knowledge { get; set; }
        public int Attitude { get; set; }
        public int Dedication { get; set; }
        public int CommunicationSkill { get; set; }
        public int Effectiveness { get; set; }
        public int Price { get; set; }
        public int ServiceSpeed { get; set; }
        public int Convenience { get; set; }
    }
    
    public class ServiceReview
    {
        public int ReviewId { get; set; }
        public int RecordId { get; set; }
        public int OverallRating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? PatientName { get; set; }
        public string? ServiceName { get; set; }
    }

    public class ServiceReviewBasic
    {
        public int ServiceId { get; set; }
        public double AvgScore { get; set; }
        public int ReviewCount { get; set; }
    }

    public class DoctorReviewBasic
    {
        public int DoctorId { get; set; }
        public double AvgScore { get; set; }
        public int ReviewCount { get; set; }
    }

    public class DoctorReviewDetailDTO
    {
        public int ReviewId { get; set; }
        public int MedicalRecordId { get; set; }
        public int OverallRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Knowledge { get; set; }
        public int Attitude { get; set; }
        public int Dedication { get; set; }
        public int CommunicationSkill { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
    }

    public class ServiceReviewDetailDTO
    {
        public int ReviewId { get; set; }
        public int MedicalRecordId { get; set; }
        public int OverallRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Effectiveness { get; set; }
        public int Price { get; set; }
        public int ServiceSpeed { get; set; }
        public int Convenience { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
    }

    public class ReviewRating
    {
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}