using server.DTO;
using server.Models;

namespace Server.Tests.Mocks
{
    /// <summary>
    /// Lớp chứa dữ liệu mock cho các Unit Test
    /// Tập trung quản lý tất cả test data ở một nơi
    /// </summary>
    public static class MockData
    {
        #region Patient Data
        
        /// <summary>
        /// Tạo một Patient hợp lệ dùng cho test
        /// </summary>
        public static PatientDTO.PatientDetail ValidPatient => new PatientDTO.PatientDetail
        {
            PatientId = 100,
            UserName = "Nguyễn Văn A",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Address = "123 Đường ABC, Quận 1, TP.HCM",
            PhoneNumber = "0901234567",
            Email = "sinluvdex1510@gmail.com"
        };

        /// <summary>
        /// Tạo một PatientBasic hợp lệ dùng cho test (cho GetPatientById)
        /// </summary>
        public static PatientDTO.PatientBasic ValidPatientBasic => new PatientDTO.PatientBasic
        {
            PatientId = 100,
            UserName = "Nguyễn Văn A",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Email = "sinluvdex1510@gmail.com"
        };

        /// <summary>
        /// Patient không có PatientId (user mới chưa có patient record)
        /// </summary>
        public static PatientDTO.PatientDetail PatientWithoutId => new PatientDTO.PatientDetail
        {
            PatientId = null,
            UserName = "Nguyễn Văn B",
            Email = "patient@test.com"
        };

        #endregion

        #region Doctor Data

        /// <summary>
        /// Tạo một Doctor hợp lệ dùng cho test
        /// </summary>
        public static DoctorDTO.DoctorDetail ValidDoctor => new DoctorDTO.DoctorDetail
        {
            DoctorId = 10,
            UserId = 2,
            UserName = "LÊ ANH TIẾN",
            SpecialtyId = 1,
            Degree = "Tiến sĩ",
            Position = "Trưởng khoa",
            ExperienceYears = 15,
            Email = "r.rhm@gmail.com",
            PhoneNumber = "0909876543"
        };

        /// <summary>
        /// List các Doctor theo chuyên khoa
        /// </summary>
        public static List<DoctorDTO.DoctorBasic> DoctorsBySpecialty => new List<DoctorDTO.DoctorBasic>
        {
            new DoctorDTO.DoctorBasic
            {
                DoctorId = 10,
                UserName = "LÊ ANH TIẾN",
                SpecialtyId = 1,
                Position = "Trưởng khoa"
            },
            new DoctorDTO.DoctorBasic
            {
                DoctorId = 11,
                UserName = "TRẦN HỮU LỢI",
                SpecialtyId = 1,
                Position = "Phó khoa"
            }
        };

        #endregion

        #region Specialty Data

        /// <summary>
        /// Specialty hợp lệ dùng cho test
        /// </summary>
        public static Specialty ValidSpecialty => new Specialty
        {
            SpecialtyId = 1,
            Name = "Khoa Răng - Hàm - Mặt",
            Description = "Chuyên khoa về răng hàm mặt"
        };

        /// <summary>
        /// List các Specialty
        /// </summary>
        public static List<Specialty> Specialties => new List<Specialty>
        {
            new Specialty { SpecialtyId = 1, Name = "Khoa Răng - Hàm - Mặt" },
            new Specialty { SpecialtyId = 2, Name = "Khoa Nội tổng quát" },
            new Specialty { SpecialtyId = 3, Name = "Khoa Tim mạch" }
        };

        #endregion

        #region Service Data

        /// <summary>
        /// Service hợp lệ dùng cho test
        /// </summary>
        public static ServiceDTO.ServiceDetail ValidService => new ServiceDTO.ServiceDetail
        {
            ServiceId = 5,
            ServiceName = "Tẩy trắng răng",
            Description = "Dịch vụ tẩy trắng răng chuyên nghiệp",
            Price = 500000
        };

        /// <summary>
        /// List các Service theo chuyên khoa
        /// </summary>
        public static List<ServiceDTO.ServiceDetail> ServicesBySpecialty => new List<ServiceDTO.ServiceDetail>
        {
            new ServiceDTO.ServiceDetail { ServiceId = 5, ServiceName = "Tẩy trắng răng", Price = 500000 },
            new ServiceDTO.ServiceDetail { ServiceId = 6, ServiceName = "Nhổ răng khôn", Price = 800000 },
            new ServiceDTO.ServiceDetail { ServiceId = 7, ServiceName = "Cạo vôi răng", Price = 200000 }
        };

        #endregion

        #region Appointment Data

        /// <summary>
        /// Appointment hợp lệ (chưa hoàn thành)
        /// </summary>
        public static Appointment ExistingUnfinishedAppointment => new Appointment
        {
            AppointmentId = 888,
            PatientId = 100,
            DoctorId = 10,
            ServiceId = 5,
            AppointmentDate = DateTime.Now.Date.AddDays(1),
            AppointmentTime = "Sáng",
            Status = "Chờ xác nhận"
        };

        /// <summary>
        /// Appointment đã hoàn thành
        /// </summary>
        public static Appointment CompletedAppointment => new Appointment
        {
            AppointmentId = 999,
            PatientId = 100,
            DoctorId = 10,
            ServiceId = 5,
            AppointmentDate = DateTime.Now.Date.AddDays(-7),
            AppointmentTime = "Sáng",
            Status = "Đã hoàn thành"
        };

        /// <summary>
        /// Appointment mới được tạo thành công
        /// </summary>
        public static Appointment NewlyCreatedAppointment => new Appointment
        {
            AppointmentId = 1000,
            PatientId = 100,
            DoctorId = 10,
            ServiceId = 5,
            AppointmentDate = DateTime.Now.Date.AddDays(3),
            AppointmentTime = "Sáng",
            Status = "Chờ xác nhận"
        };

        #endregion

        #region AppointmentForm Data

        /// <summary>
        /// Form đặt lịch hợp lệ với tất cả thông tin
        /// </summary>
        public static Server.DTO.AppointmentForm ValidAppointmentForm => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form thiếu Department (DLK03)
        /// </summary>
        public static Server.DTO.AppointmentForm FormMissingDepartment => new Server.DTO.AppointmentForm
        {
            Department = "",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form thiếu Doctor (DLK04)
        /// </summary>
        public static Server.DTO.AppointmentForm FormMissingDoctor => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form thiếu Service (DLK05)
        /// </summary>
        public static Server.DTO.AppointmentForm FormMissingService => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form thiếu AppointmentDate (DLK06)
        /// </summary>
        public static Server.DTO.AppointmentForm FormMissingDate => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = "",
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form với ngày trong quá khứ (DLK07)
        /// </summary>
        public static Server.DTO.AppointmentForm FormPastDate => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form thiếu AppointmentTime (DLK08)
        /// </summary>
        public static Server.DTO.AppointmentForm FormMissingTime => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "",
            Symptoms = "Vàng ố"
        };

        /// <summary>
        /// Form với Symptoms quá dài (>500 ký tự)
        /// </summary>
        public static Server.DTO.AppointmentForm FormSymptomsTooLong => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(3).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = new string('A', 501) // 501 ký tự
        };

        /// <summary>
        /// Form với ngày quá xa (>15 ngày)
        /// </summary>
        public static Server.DTO.AppointmentForm FormDateTooFar => new Server.DTO.AppointmentForm
        {
            Department = "Khoa Răng - Hàm - Mặt",
            Doctor = "LÊ ANH TIẾN",
            Service = "Tẩy trắng răng",
            AppointmentDate = DateTime.Now.Date.AddDays(20).ToString("yyyy-MM-dd"),
            AppointmentTime = "Sáng",
            Symptoms = "Vàng ố"
        };

        #endregion

        #region User Claims Data

        /// <summary>
        /// User ID của Patient dùng cho test
        /// </summary>
        public const int PatientUserId = 1;

        /// <summary>
        /// User ID của Doctor dùng cho test
        /// </summary>
        public const int DoctorUserId = 2;

        /// <summary>
        /// User ID của Admin dùng cho test
        /// </summary>
        public const int AdminUserId = 3;

        #endregion

        #region Prescription Data (Kê đơn thuốc - KDT01-KDT10)

        /// <summary>
        /// Medicine hợp lệ - Paracetamol
        /// </summary>
        public static Medicine ValidMedicine => new Medicine
        {
            MedicineId = 1,
            MedicalName = "Paracetamol(500mg/l)",
            StockQuantity = 1000,
            ExpiredDate = DateTime.Now.AddYears(2),
            Unit = "viên",
            Price = 5000
        };

        /// <summary>
        /// Danh sách thuốc cho test
        /// </summary>
        public static List<Medicine> MedicineList => new List<Medicine>
        {
            new Medicine
            {
                MedicineId = 1,
                MedicalName = "Paracetamol(500mg/l)",
                StockQuantity = 1000,
                ExpiredDate = DateTime.Now.AddYears(2),
                Unit = "viên",
                Price = 5000
            },
            new Medicine
            {
                MedicineId = 2,
                MedicalName = "Amoxicillin(250mg)",
                StockQuantity = 500,
                ExpiredDate = DateTime.Now.AddYears(1),
                Unit = "viên",
                Price = 8000
            },
            new Medicine
            {
                MedicineId = 3,
                MedicalName = "Ibuprofen(400mg)",
                StockQuantity = 800,
                ExpiredDate = DateTime.Now.AddYears(1).AddMonths(6),
                Unit = "viên",
                Price = 6000
            }
        };

        /// <summary>
        /// Appointment hôm nay để kê đơn thuốc (KDT01 - thành công)
        /// </summary>
        public static Appointment TodayAppointmentForPrescription => new Appointment
        {
            AppointmentId = 500,
            PatientId = 100,
            DoctorId = 10,
            ServiceId = 5,
            AppointmentDate = DateTime.Now.Date,
            AppointmentTime = "Sáng",
            Status = "Chờ khám"
        };

        /// <summary>
        /// Appointment ngày khác (không phải hôm nay - KDT09)
        /// </summary>
        public static Appointment FutureAppointmentForPrescription => new Appointment
        {
            AppointmentId = 501,
            PatientId = 100,
            DoctorId = 10,
            ServiceId = 5,
            AppointmentDate = new DateTime(2026, 2, 1), // Ngày 1/2/2026
            AppointmentTime = "Sáng",
            Status = "Chờ khám"
        };

        /// <summary>
        /// Appointment của bác sĩ khác (không có quyền truy cập)
        /// </summary>
        public static Appointment OtherDoctorAppointment => new Appointment
        {
            AppointmentId = 502,
            PatientId = 100,
            DoctorId = 20, // Bác sĩ khác
            ServiceId = 5,
            AppointmentDate = DateTime.Now.Date,
            AppointmentTime = "Sáng",
            Status = "Chờ khám"
        };

        /// <summary>
        /// PrescriptionRequest hợp lệ với đầy đủ thông tin (KDT01)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest ValidPrescriptionRequest => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau khi ăn 1 giờ",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "1",
                    FrequencyPerDay = "3",
                    DurationInDays = "3",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 9, // 1 * 3 * 3 = 9
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest không có thuốc (KDT02)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionWithoutMedicine => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Viêm lợi",
            Treatment = "Theo dõi thêm",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>() // Rỗng
        };

        /// <summary>
        /// PrescriptionRequest với thuốc thiếu thông tin bắt buộc (KDT03)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionMissingMedicineInfo => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "", // Thiếu liều dùng
                    FrequencyPerDay = "", // Thiếu số lần/ngày
                    DurationInDays = "", // Thiếu số ngày
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 0,
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest với liều dùng = 0 (KDT04)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionZeroDosage => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "0", // Liều dùng = 0
                    FrequencyPerDay = "3",
                    DurationInDays = "3",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 0,
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest với nhiều thuốc (KDT05)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionMultipleMedicines => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "1",
                    FrequencyPerDay = "3",
                    DurationInDays = "3",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 9,
                    Price = 5000
                },
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 2,
                    MedicineName = "Amoxicillin(250mg)",
                    Dosage = "2",
                    FrequencyPerDay = "2",
                    DurationInDays = "5",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 20, // 2 * 2 * 5 = 20
                    Price = 8000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest không có lưu ý bổ sung (KDT06)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionWithoutNotes => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "", // Không có lưu ý
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "1",
                    FrequencyPerDay = "3",
                    DurationInDays = "3",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 9,
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest để test tự động tính tổng số lượng (KDT07)
        /// Liều dùng: 1, Số lần/ngày: 3, Số ngày: 5 => Tổng = 15
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionAutoCalculateQuantity => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "1",
                    FrequencyPerDay = "3",
                    DurationInDays = "5",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 15, // 1 * 3 * 5 = 15
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// PrescriptionRequest thiếu chẩn đoán bệnh (KDT10)
        /// </summary>
        public static server.DTO.MedicalRecordDTO.PrescriptionRequest PrescriptionMissingDiagnosis => new server.DTO.MedicalRecordDTO.PrescriptionRequest
        {
            Diagnosis = "", // Thiếu chẩn đoán
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau ăn",
            Medicines = new List<server.DTO.MedicalRecordDTO.MedicineDto>
            {
                new server.DTO.MedicalRecordDTO.MedicineDto
                {
                    MedicineId = 1,
                    MedicineName = "Paracetamol(500mg/l)",
                    Dosage = "1",
                    FrequencyPerDay = "3",
                    DurationInDays = "3",
                    Usage = "Uống sau ăn",
                    Unit = "viên",
                    Quantity = 9,
                    Price = 5000
                }
            }
        };

        /// <summary>
        /// MedicalRecord mới được tạo thành công
        /// </summary>
        public static MedicalRecord NewlyCreatedMedicalRecord => new MedicalRecord
        {
            RecordId = 1,
            AppointmentId = 500,
            Diagnosis = "Sâu răng nhẹ",
            Treatment = "Vệ sinh răng miệng",
            Notes = "Uống sau khi ăn 1 giờ",
            CreatedAt = DateTime.Now
        };

        /// <summary>
        /// MedicalRecordDetail cho test
        /// </summary>
        public static List<MedicalRecordDetail> ValidMedicalRecordDetails => new List<MedicalRecordDetail>
        {
            new MedicalRecordDetail
            {
                MedicineId = 1,
                ReCordId = 1,
                Quantity = 9,
                Dosage = 1,
                FrequencyPerDay = 3,
                DurationInDays = 3,
                Usage = "Uống sau ăn"
            }
        };

        /// <summary>
        /// MedicineDto list for GetRecordDetail response
        /// </summary>
        public static List<server.DTO.MedicalRecordDTO.MedicineDto> MedicineDtoList => new List<server.DTO.MedicalRecordDTO.MedicineDto>
        {
            new server.DTO.MedicalRecordDTO.MedicineDto
            {
                MedicineId = 1,
                MedicineName = "Paracetamol(500mg/l)",
                Dosage = "1",
                FrequencyPerDay = "3",
                DurationInDays = "3",
                Usage = "Uống sau ăn",
                Unit = "viên",
                Quantity = 9,
                Price = 5000
            }
        };

        #endregion
    }
}
