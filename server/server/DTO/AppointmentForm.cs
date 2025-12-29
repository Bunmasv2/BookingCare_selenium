namespace Server.DTO
{
    public class AppointmentForm
    {
        public string Department { get; set; } // Khoa
        public string Doctor { get; set; } // Tên bác sĩ
        public string AppointmentDate { get; set; } // Ngày khám
        public string AppointmentTime { get; set; } // Buổi khám (Sáng/Chiều)
        public string Symptoms { get; set; } // Triệu chứng
        public string Service {get; set;} // Tên dịch vụ
    }
}