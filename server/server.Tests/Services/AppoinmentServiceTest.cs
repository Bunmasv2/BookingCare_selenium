using Moq;
using NUnit.Framework;
using server.DTO;
using server.Models;
using server.Services;
using Server.Tests.Mocks;

namespace Server.Tests.Services
{
    /// <summary>
    /// Unit Tests cho AppointmentService (Service Layer)
    /// Các test này kiểm tra business logic trong Service layer
    /// </summary>
    [TestFixture]
    public class AppointmentServiceTest
    {
        private Mock<IAppointment> _mockAppointmentService;

        [SetUp]
        public void Setup()
        {
            _mockAppointmentService = new Mock<IAppointment>();
        }

        #region IsExistAppointment Tests

        /// <summary>
        /// Test: Không tìm thấy lịch hẹn chưa hoàn thành -> Return null
        /// </summary>
        [Test]
        public async Task IsExistAppointment_NoUnfinishedAppointment_ReturnsNull()
        {
            // Arrange
            int patientId = MockData.PatientUserId;
            DateTime appointmentDate = DateTime.Now.AddDays(3);
            string appointmentTime = "Sáng";

            _mockAppointmentService
                .Setup(s => s.IsExistAppointment(patientId, appointmentDate, appointmentTime))
                .ReturnsAsync((Appointment)null!);

            // Act
            var result = await _mockAppointmentService.Object.IsExistAppointment(patientId, appointmentDate, appointmentTime);

            // Assert
            Assert.That(result, Is.Null, "Khi không có lịch hẹn chưa hoàn thành, phải trả về null");
        }

        /// <summary>
        /// Test: Tìm thấy lịch hẹn chưa hoàn thành -> Return Appointment object
        /// </summary>
        [Test]
        public async Task IsExistAppointment_HasUnfinishedAppointment_ReturnsAppointment()
        {
            // Arrange
            int patientId = 100;
            DateTime appointmentDate = DateTime.Now.AddDays(1);
            string appointmentTime = "Sáng";

            var existingAppointment = MockData.ExistingUnfinishedAppointment;

            _mockAppointmentService
                .Setup(s => s.IsExistAppointment(patientId, It.IsAny<DateTime>(), appointmentTime))
                .ReturnsAsync(existingAppointment);

            // Act
            var result = await _mockAppointmentService.Object.IsExistAppointment(patientId, appointmentDate, appointmentTime);

            // Assert
            Assert.That(result, Is.Not.Null, "Khi có lịch hẹn chưa hoàn thành, phải trả về đối tượng Appointment");
            Assert.That(result!.Status, Is.EqualTo("Chờ xác nhận"), "Trạng thái phải là 'Chờ xác nhận'");
            Assert.That(result.PatientId, Is.EqualTo(patientId), "PatientId phải khớp");
        }

        #endregion

        #region Appointment (Create) Tests

        /// <summary>
        /// Test: Tạo lịch hẹn thành công
        /// </summary>
        [Test]
        public async Task Appointment_ValidData_ReturnsNewAppointment()
        {
            // Arrange
            int patientId = 100;
            int doctorId = 10;
            int serviceId = 5;
            DateTime appointmentDate = DateTime.Now.AddDays(3);
            string appointmentTime = "Sáng";
            string status = "Chờ xác nhận";

            var newAppointment = new Appointment
            {
                AppointmentId = 1000,
                PatientId = patientId,
                DoctorId = doctorId,
                ServiceId = serviceId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                Status = status
            };

            _mockAppointmentService
                .Setup(s => s.Appointment(patientId, doctorId, serviceId, appointmentDate, appointmentTime, status))
                .ReturnsAsync(newAppointment);

            // Act
            var result = await _mockAppointmentService.Object.Appointment(
                patientId, doctorId, serviceId, appointmentDate, appointmentTime, status);

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về đối tượng Appointment mới");
            Assert.That(result.AppointmentId, Is.GreaterThan(0), "AppointmentId phải > 0");
            Assert.That(result.Status, Is.EqualTo("Chờ xác nhận"), "Trạng thái ban đầu phải là 'Chờ xác nhận'");
        }

        #endregion

        #region CountAppointsByDate Tests

        /// <summary>
        /// Test: Đếm số lịch hẹn trong ngày - ít hơn 15
        /// </summary>
        [Test]
        public async Task CountAppointsByDate_LessThan15_ReturnsCount()
        {
            // Arrange
            DateTime date = DateTime.Now.AddDays(3);
            string time = "Sáng";

            _mockAppointmentService
                .Setup(s => s.CountAppointsByDate(date, time))
                .ReturnsAsync(10);

            // Act
            var count = await _mockAppointmentService.Object.CountAppointsByDate(date, time);

            // Assert
            Assert.That(count, Is.LessThanOrEqualTo(15), "Số lịch hẹn phải <= 15");
        }

        /// <summary>
        /// Test: Đếm số lịch hẹn trong ngày - quá 15
        /// </summary>
        [Test]
        public async Task CountAppointsByDate_MoreThan15_ReturnsCount()
        {
            // Arrange
            DateTime date = DateTime.Now.AddDays(3);
            string time = "Sáng";

            _mockAppointmentService
                .Setup(s => s.CountAppointsByDate(date, time))
                .ReturnsAsync(16);

            // Act
            var count = await _mockAppointmentService.Object.CountAppointsByDate(date, time);

            // Assert
            Assert.That(count, Is.GreaterThan(15), "Số lịch hẹn phải > 15");
        }

        #endregion

        #region GetAppointments Tests

        /// <summary>
        /// Test: Lấy danh sách lịch hẹn
        /// </summary>
        [Test]
        public async Task GetAppointments_ReturnsListOfAppointments()
        {
            // Arrange
            var appointmentsList = new List<AppointmentDTO.AppointmentDetail>
            {
                new AppointmentDTO.AppointmentDetail
                {
                    AppointmentId = 1,
                    PatientName = "Nguyễn Văn A",
                    DoctorName = "LÊ ANH TIẾN",
                    ServiceName = "Tẩy trắng răng",
                    AppointmentDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    AppointmentTime = "Sáng",
                    Status = "Chờ xác nhận"
                },
                new AppointmentDTO.AppointmentDetail
                {
                    AppointmentId = 2,
                    PatientName = "Trần Văn B",
                    DoctorName = "TRẦN HỮU LỢI",
                    ServiceName = "Nhổ răng khôn",
                    AppointmentDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"),
                    AppointmentTime = "Chiều",
                    Status = "Đã xác nhận"
                }
            };

            _mockAppointmentService
                .Setup(s => s.GetAppointments())
                .ReturnsAsync(appointmentsList);

            // Act
            var result = await _mockAppointmentService.Object.GetAppointments();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        #endregion

        #region GetAppointmentsByMonthYear Tests

        /// <summary>
        /// Test: Lấy danh sách lịch hẹn theo tháng/năm
        /// </summary>
        [Test]
        public async Task GetAppointmentsByMonthYear_ValidMonthYear_ReturnsFilteredList()
        {
            // Arrange
            int month = 12;
            int year = 2025;

            var appointmentsList = new List<AppointmentDTO.AppointmentDetail>
            {
                new AppointmentDTO.AppointmentDetail
                {
                    AppointmentId = 1,
                    PatientName = "Nguyễn Văn A",
                    AppointmentDate = "2025-12-15",
                    Status = "Chờ xác nhận"
                }
            };

            _mockAppointmentService
                .Setup(s => s.GetAppointmentsByMonthYear(month, year))
                .ReturnsAsync(appointmentsList);

            // Act
            var result = await _mockAppointmentService.Object.GetAppointmentsByMonthYear(month, year);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        #endregion

        #region UpdateStatus Tests

        /// <summary>
        /// Test: Cập nhật trạng thái lịch hẹn thành công
        /// </summary>
        [Test]
        public async Task UpdateStatus_ValidStatus_UpdatesSuccessfully()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentId = 1,
                Status = "Chờ xác nhận"
            };
            string newStatus = "Đã xác nhận";

            _mockAppointmentService
                .Setup(s => s.UpdateStatus(appointment, newStatus))
                .Returns(Task.CompletedTask);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => 
                await _mockAppointmentService.Object.UpdateStatus(appointment, newStatus));
        }

        #endregion

        #region AppointmentStatistics Tests

        /// <summary>
        /// Test: Thống kê lịch hẹn theo tháng/năm
        /// </summary>
        [Test]
        public async Task AppointmentStatistics_ValidMonthYear_ReturnsStatistics()
        {
            // Arrange
            int month = 12;
            int year = 2025;

            var statistics = new List<AppointmentDTO.AppointmentGroup>
            {
                new AppointmentDTO.AppointmentGroup { Status = "Chờ xác nhận", Appointments = 5 },
                new AppointmentDTO.AppointmentGroup { Status = "Đã xác nhận", Appointments = 3 },
                new AppointmentDTO.AppointmentGroup { Status = "Đã khám", Appointments = 10 },
                new AppointmentDTO.AppointmentGroup { Status = "Đã hoàn thành", Appointments = 15 },
                new AppointmentDTO.AppointmentGroup { Status = "Đã hủy", Appointments = 2 }
            };

            _mockAppointmentService
                .Setup(s => s.AppointmentStatistics(month, year))
                .ReturnsAsync(statistics);

            // Act
            var result = await _mockAppointmentService.Object.AppointmentStatistics(month, year);

            // Assert
            Assert.That(result, Is.Not.Null);
            var resultList = result as List<AppointmentDTO.AppointmentGroup>;
            Assert.That(resultList!.Count, Is.EqualTo(5), "Phải có 5 loại trạng thái");
        }

        #endregion
    }
}
