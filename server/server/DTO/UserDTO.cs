namespace server.DTO
{
    public class UserDTO
    {
        public class UserBasic
        {
            public int Id { get; set; }
            public string? UserName { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }
        }

        public class Doctor
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public int DoctorId { get; set; }
            public string? SpecialtyName { get; set; }
            public int? ExperienceYears { get; set; }
            public string? Position { get; set; }
            public string? Biography { get; set; }
            public string? Qualifications { get; set; }
            public string? WorkExperience { get; set; }
            public string? Degree { get; set; }
            public string? DoctorImage { get; set; }
        }

        public class Patient
        {
            public int UserId { get; set; }
            public int PatientId { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public class Admin
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public string? PhoneNumber { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Address { get; set; }
        }
    }
}
