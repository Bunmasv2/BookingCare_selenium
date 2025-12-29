namespace server.DTO
{
    public class AppointmentDTO
    {
        public class DoctorScheduleDTO
        {
            public DateTime Date { get; set; }
            public string AppointmentTime { get; set; }
            public int PatientCount { get; set; } // <- dòng này phải tồn tại!

        }

        public class AppointmentDetail
        {
            public int? AppointmentId { get; set; }
            public string? PatientName { get; set; }
            public string? DoctorName { get; set; }
            public string? ServiceName { get; set; }
            public string? AppointmentDate { get; set; }
            public string? AppointmentTime { get; set; }
            public string? Status { get; set; }
        }

        public class PatientUpdateDTO
        {
            public string? Address { get; set; }
            public DateOnly? DateOfBirth { get; set; }
        }

        public class AppointmentGroup
        {
            public string Status { get; set; }
            public int Appointments { get; set; }
        }

        public class AvailableAppointment
        {
            public DateTime Date { get; set; }
            public string Time { get; set; }
            // public int Quantity { get; set; }
        }
    }
}
