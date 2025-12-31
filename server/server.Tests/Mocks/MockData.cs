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
    }
}
