namespace server.DTO
{
    public class DoctorDTO
    {
        public class DoctorBasic
        {
            public int? DoctorId { get; set; }
            public int? SpecialtyId { get; set; }
            public string? UserName { get; set; }
            public string? Position { get; set; }
            public int? ExperienceYears { get; set; }
            public string? DoctorImage { get; set; }
            public string? Degree { get; set; }
        }

        public class DoctorDetail
        {
            public int? DoctorId { get; set; }
            public int? SpecialtyId { get; set; }
            public int? UserId { get; set; }
            public string? UserName { get; set; }
            public string? Degree { get; set; }
            public string? Position { get; set; }
            public string? Biography { get; set; }
            public string? Qualifications { get; set; }
            public string? WorkExperience { get; set; }
            public int? ExperienceYears { get; set; }
            public string? DoctorImage { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }
        
        public class DoctorSalaryDTO
        {
            public int DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Specialty { get; set; }
            public int AppointmentCount { get; set; }
            public decimal TotalServicePrice { get; set; }
            public decimal BaseSalary { get; set; } = 4000000;
            public decimal Commission { get; set; } // ví dụ: 30% của dịch vụ
            public decimal Bonus { get; set; } // thưởng theo top
            public decimal TotalSalary => BaseSalary + Commission + Bonus;
        }

        public class DoctorSalaryDetailDTO
        {
            public string PatientName { get; set; }
            public string ServiceName { get; set; }
            public decimal ServicePrice { get; set; }
            public DateTime AppointmentDate { get; set; }
            public decimal Commission { get; set; }

        }
        public class DoctorSalaryDetailResultDTO
        {
            public List<DoctorSalaryDetailDTO> Details { get; set; }
            public decimal BaseSalary { get; set; }
            public decimal Bonus { get; set; }
            public decimal CommissionTotal { get; set; }
            public decimal TotalSalary { get; set; }
        }


    }
}
