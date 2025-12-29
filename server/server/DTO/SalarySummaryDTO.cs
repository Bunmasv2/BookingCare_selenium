namespace server.DTO
{
    public class SalarySummaryDTO
    {
        public decimal TotalCommission { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public List<DoctorDTO.DoctorSalaryDTO> DoctorSalaries { get; set; }
    }
}
