namespace server.DTO
{
    public class MedicalRecordDTO
    {
        public class PrescriptionRequest
        {
            public string Diagnosis { get; set; }
            public string Treatment { get; set; }
            public string Notes { get; set; } = "Uống sau khi ăn";
            public List<MedicineDto> Medicines { get; set; }
        }

        public class MedicalRecordBasic
        {
            public int RecordId { get; set; }
            public string Diagnosis { get; set; }
            public string Treatment { get; set; }
            public string Notes { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
            public string DoctorName { get; set; }
            public string PatientName { get; set; }
            public int PatientId { get; set; }
            public string SpecialtyName { get; set; }
            public string ServiceName { get; set; }
            public string? Status { get; set; }
        }

        public class MedicineDto
        {
            public int MedicineId { get; set; }
            public string MedicineName { get; set; }
            public string Dosage { get; set; }
            public string FrequencyPerDay { get; set; }
            public string DurationInDays { get; set; }
            public string Usage { get; set; }
            public string Unit { get; set; }
            public int Quantity { get; set; }
            public float Price { get; set; }

        }
        public class CheckPaymentStatusResponse
        {
            public int ResultCode { get; set; }
            public string Message { get; set; }
            public string OrderId { get; set; }
            public string RequestId { get; set; }
            public long Amount { get; set; }
            public string OrderInfo { get; set; }
        }
        // Request DTO// Update CreatePaymentRequest DTO to only include OrderInfo
        public class CreatePaymentRequest
        {
            public string OrderInfo { get; set; }

            public int RecordId { get; set; }
        }
    }
}
