namespace server.DTO
{
    public class PatientDTO
    {
        public class PatientBasic
        {
            public int? PatientId { get; set; }
            public string? UserName { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Email { get; set; }
        }

        public class PatientDetail
        {
            public int? PatientId { get; set; }
            public string? UserName { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }
        }
        public class PatientUpdateDTO
        {
            public string? Address { get; set; }
            public DateOnly? DateOfBirth { get; set; }
        }
    }
}
