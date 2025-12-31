using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using server.Controllers;
using server.Services;
using server.Models;
using server.DTO;
using server.Middleware;
using Microsoft.Extensions.Configuration;
using Server.DTO;
using Server.Tests.Mocks;

namespace Server.Tests.Controllers
{
    /// <summary>
    /// Unit Tests cho AppointmentsController
    /// Dựa theo các Test Cases từ file Excel: DLK01 - DLK10
    /// Sử dụng Moq để mock các dependencies
    /// </summary>
    [TestFixture]
    public class AppointmentsControllerTests
    {
        #region Private Fields - Mock Objects

        private Mock<IAppointment> _mockAppointmentService = null!;
        private Mock<IDoctor> _mockDoctorService = null!;
        private Mock<IPatient> _mockPatientService = null!;
        private Mock<IService> _mockServiceService = null!;
        private Mock<ISpecialty> _mockSpecialtyService = null!;
        private Mock<IConfiguration> _mockConfig = null!;
        private AppointmentsController _controller = null!;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            // Khởi tạo các Mock
            _mockAppointmentService = new Mock<IAppointment>();
            _mockDoctorService = new Mock<IDoctor>();
            _mockPatientService = new Mock<IPatient>();
            _mockServiceService = new Mock<IService>();
            _mockSpecialtyService = new Mock<ISpecialty>();
            _mockConfig = new Mock<IConfiguration>();

            // Inject Mocks vào Controller
            _controller = new AppointmentsController(
                null!, // Context (không cần vì dùng Service)
                _mockDoctorService.Object,
                _mockPatientService.Object,
                _mockAppointmentService.Object,
                _mockSpecialtyService.Object,
                _mockServiceService.Object,
                _mockConfig.Object
            );

            // Giả lập User đã đăng nhập với Role = Patient
            SetupUserContext(MockData.PatientUserId, "patient");
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        /// <summary>
        /// Helper method để setup HttpContext với User Claims
        /// </summary>
        private void SetupUserContext(int userId, string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        /// <summary>
        /// Helper để setup các mocks chuẩn cho test thành công
        /// </summary>
        private void SetupSuccessfulMocks()
        {
            // Mock Patient
            _mockPatientService
                .Setup(p => p.GetPatientByUserId(MockData.PatientUserId))
                .ReturnsAsync(MockData.ValidPatient);

            // Mock Specialty
            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            // Mock Doctor
            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidDoctor);

            // Mock Service
            _mockServiceService
                .Setup(s => s.GetServiceByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidService);

            // Mock không có lịch hẹn chưa hoàn thành
            _mockAppointmentService
                .Setup(s => s.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync((Appointment)null!);

            // Mock đếm số lịch hẹn (ít hơn 15)
            _mockAppointmentService
                .Setup(s => s.CountAppointsByDate(It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(5);

            // Mock tạo lịch hẹn thành công
            _mockAppointmentService
                .Setup(s => s.Appointment(
                    It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                    It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MockData.NewlyCreatedAppointment);
        }

        #endregion

        #region DLK01 - Đặt lịch thành công với thông tin hợp lệ

        /// <summary>
        /// DLK01: Kiểm tra đăng nhập thành công với thông tin hợp lệ
        /// Expected: Hiển thị thông báo "Đặt lịch thành công"
        /// </summary>
        [Test]
        [Category("DLK01")]
        [Description("Kiểm tra đặt lịch thành công với thông tin hợp lệ")]
        public async Task Appointment_DLK01_ValidData_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            SetupSuccessfulMocks();

            var form = MockData.ValidAppointmentForm;

            // Act
            var result = await _controller.Appointment(form) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Kết quả phải là OkObjectResult");
            Assert.That(result!.StatusCode, Is.EqualTo(200), "Status code phải là 200");
            
            var response = result.Value;
            var messageProperty = response?.GetType().GetProperty("message");
            var message = messageProperty?.GetValue(response)?.ToString();
            
            Assert.That(message, Does.Contain("thành công"), "Thông báo phải chứa từ 'thành công'");
        }

        #endregion

        #region DLK02 - User chưa đăng nhập (Authorize attribute handles this)

        /// <summary>
        /// DLK02: Kiểm tra đặt lịch khi user chưa đăng nhập
        /// Expected: Chuyển đến trang đăng nhập (401 Unauthorized)
        /// Note: Được xử lý bởi [Authorize] attribute ở Controller level
        /// Khi không có user claim, Controller throw ErrorHandlingException
        /// </summary>
        [Test]
        [Category("DLK02")]
        [Description("Kiểm tra đặt lịch khi user chưa đăng nhập")]
        public void Appointment_DLK02_Unauthorized_ThrowsException()
        {
            // Arrange - Setup user không có NameIdentifier claim
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // Empty identity
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            // Controller sẽ throw ErrorHandlingException khi không tìm được Patient
            // vì GetPatientByUserId(0) sẽ trả về null
            _mockPatientService
                .Setup(p => p.GetPatientByUserId(It.IsAny<int>()))
                .ReturnsAsync((PatientDTO.PatientDetail)null!);

            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidDoctor);

            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));
            
            Assert.That(exception!.StatusCode, Is.EqualTo(404));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Không tìm thấy bệnh nhân"));
        }

        #endregion

        #region DLK03 - Không chọn Chuyên khoa

        /// <summary>
        /// DLK03: Kiểm tra đặt lịch thất bại khi không chọn Chuyên khoa
        /// Expected: "Vui lòng chọn khoa"
        /// </summary>
        [Test]
        [Category("DLK03")]
        [Description("Kiểm tra đặt lịch thất bại khi không chọn Chuyên khoa")]
        public void Appointment_DLK03_MissingDepartment_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormMissingDepartment;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Vui lòng chọn khoa"));
        }

        #endregion

        #region DLK04 - Không chọn Bác sĩ

        /// <summary>
        /// DLK04: Kiểm tra đặt lịch thất bại khi không chọn Bác sĩ
        /// Expected: "Vui lòng chọn bác sĩ"
        /// </summary>
        [Test]
        [Category("DLK04")]
        [Description("Kiểm tra đặt lịch thất bại khi không chọn Bác sĩ")]
        public void Appointment_DLK04_MissingDoctor_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormMissingDoctor;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Vui lòng chọn bác sĩ"));
        }

        #endregion

        #region DLK05 - Không chọn Dịch vụ

        /// <summary>
        /// DLK05: Kiểm tra đặt lịch thất bại khi không chọn Dịch vụ
        /// Expected: "Vui lòng chọn dịch vụ"
        /// </summary>
        [Test]
        [Category("DLK05")]
        [Description("Kiểm tra đặt lịch thất bại khi không chọn Dịch vụ")]
        public void Appointment_DLK05_MissingService_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormMissingService;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Vui lòng chọn dịch vụ"));
        }

        #endregion

        #region DLK06 - Không chọn Ngày khám

        /// <summary>
        /// DLK06: Kiểm tra đặt lịch thất bại khi không chọn Ngày khám
        /// Expected: "Vui lòng chọn ngày khám"
        /// </summary>
        [Test]
        [Category("DLK06")]
        [Description("Kiểm tra đặt lịch thất bại khi không chọn Ngày khám")]
        public void Appointment_DLK06_MissingDate_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormMissingDate;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Vui lòng chọn ngày khám"));
        }

        #endregion

        #region DLK07 - Ngày khám trong quá khứ / không hợp lệ

        /// <summary>
        /// DLK07: Kiểm tra đặt lịch thất bại với ngày khám trong quá khứ
        /// Expected: "Vui lòng đặt lịch khám tối thiểu trước 1 ngày"
        /// Note: Controller check ngày phải cách ít nhất 1 ngày so với hôm nay
        /// </summary>
        [Test]
        [Category("DLK07")]
        [Description("Kiểm tra đặt lịch thất bại với ngày khám trong quá khứ")]
        public void Appointment_DLK07_PastDate_ThrowsErrorHandlingException()
        {
            // Arrange
            SetupSuccessfulMocks();

            // Tạo form với ngày hôm nay (không hợp lệ vì phải ít nhất 1 ngày)
            var form = new AppointmentForm
            {
                Department = "Khoa Răng - Hàm - Mặt",
                Doctor = "LÊ ANH TIẾN",
                Service = "Tẩy trắng răng",
                AppointmentDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), // Ngày hôm nay
                AppointmentTime = "Sáng",
                Symptoms = "Vàng ố"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Does.Contain("tối thiểu trước 1 ngày"));
        }

        /// <summary>
        /// DLK07b: Kiểm tra đặt lịch thất bại với ngày khám quá xa (>15 ngày)
        /// </summary>
        [Test]
        [Category("DLK07")]
        [Description("Kiểm tra đặt lịch thất bại với ngày khám quá xa (>15 ngày)")]
        public void Appointment_DLK07b_DateTooFar_ThrowsErrorHandlingException()
        {
            // Arrange
            SetupSuccessfulMocks();
            var form = MockData.FormDateTooFar;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Does.Contain("15 ngày"));
        }

        #endregion

        #region DLK08 - Không chọn Buổi khám

        /// <summary>
        /// DLK08: Kiểm tra đặt lịch thất bại khi không chọn Buổi khám
        /// Expected: "Vui lòng chọn buổi khám"
        /// </summary>
        [Test]
        [Category("DLK08")]
        [Description("Kiểm tra đặt lịch thất bại khi không chọn Buổi khám")]
        public void Appointment_DLK08_MissingTime_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormMissingTime;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Vui lòng chọn buổi khám"));
        }

        #endregion

        #region DLK09 - Còn lịch khám chưa hoàn thành

        /// <summary>
        /// DLK09: Kiểm tra đặt lịch thất bại khi user còn lịch khám chưa hoàn thành
        /// Expected: "Bạn chưa hoàn thành lịch hẹn..."
        /// </summary>
        [Test]
        [Category("DLK09")]
        [Description("Kiểm tra đặt lịch thất bại khi user còn lịch khám chưa hoàn thành")]
        public void Appointment_DLK09_HasUnfinishedAppointment_ThrowsErrorHandlingException()
        {
            // Arrange
            _mockPatientService
                .Setup(p => p.GetPatientByUserId(MockData.PatientUserId))
                .ReturnsAsync(MockData.ValidPatient);

            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidDoctor);

            _mockServiceService
                .Setup(s => s.GetServiceByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidService);

            // Mock có lịch hẹn chưa hoàn thành
            _mockAppointmentService
                .Setup(s => s.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(MockData.ExistingUnfinishedAppointment);

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Does.Contain("Bạn chưa hoàn thành lịch hẹn"));
        }

        #endregion

        #region DLK10 - Triệu chứng quá dài

        /// <summary>
        /// DLK10: Kiểm tra đặt lịch thất bại khi Triệu chứng quá dài (>500 ký tự)
        /// Expected: "Triệu chứng quá dài"
        /// Note: Controller KHÔNG bắt buộc nhập Triệu chứng, chỉ validate độ dài nếu có nhập
        /// </summary>
        [Test]
        [Category("DLK10")]
        [Description("Kiểm tra đặt lịch thất bại khi Triệu chứng quá dài (>500 ký tự)")]
        public void Appointment_DLK10_SymptomsTooLong_ThrowsErrorHandlingException()
        {
            // Arrange
            var form = MockData.FormSymptomsTooLong;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Triệu chứng quá dài"));
        }

        #endregion

        #region Additional Tests - Edge Cases

        /// <summary>
        /// Test: Không tìm thấy khoa trong database
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Không tìm thấy khoa trong database")]
        public void Appointment_DepartmentNotFound_ThrowsErrorHandlingException()
        {
            // Arrange
            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync((Specialty)null!);

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(404));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Không tìm thấy khoa"));
        }

        /// <summary>
        /// Test: Không tìm thấy bác sĩ trong database
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Không tìm thấy bác sĩ trong database")]
        public void Appointment_DoctorNotFound_ThrowsErrorHandlingException()
        {
            // Arrange
            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync((DoctorDTO.DoctorDetail)null!);

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(404));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Không tìm thấy bác sĩ"));
        }

        /// <summary>
        /// Test: Không tìm thấy bệnh nhân
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Không tìm thấy bệnh nhân")]
        public void Appointment_PatientNotFound_ThrowsErrorHandlingException()
        {
            // Arrange
            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidDoctor);

            _mockPatientService
                .Setup(p => p.GetPatientByUserId(It.IsAny<int>()))
                .ReturnsAsync((PatientDTO.PatientDetail)null!);

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(404));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Không tìm thấy bệnh nhân"));
        }

        /// <summary>
        /// Test: Không tìm thấy dịch vụ
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Không tìm thấy dịch vụ")]
        public void Appointment_ServiceNotFound_ThrowsErrorHandlingException()
        {
            // Arrange
            _mockSpecialtyService
                .Setup(s => s.GetSpecialty(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidSpecialty);

            _mockDoctorService
                .Setup(d => d.GetDoctorByName(It.IsAny<string>()))
                .ReturnsAsync(MockData.ValidDoctor);

            _mockPatientService
                .Setup(p => p.GetPatientByUserId(It.IsAny<int>()))
                .ReturnsAsync(MockData.ValidPatient);

            _mockServiceService
                .Setup(s => s.GetServiceByName(It.IsAny<string>()))
                .ReturnsAsync((ServiceDTO.ServiceDetail)null!);

            var form = MockData.ValidAppointmentForm;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form));

            Assert.That(exception!.StatusCode, Is.EqualTo(404));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Không tìm thấy dịch vụ"));
        }

        /// <summary>
        /// Test: Quá 15 lịch hẹn trong ngày - trả về available slots
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Quá 15 lịch hẹn trong ngày - trả về available slots")]
        public async Task Appointment_TooManyAppointments_ReturnsAvailableSlots()
        {
            // Arrange
            SetupSuccessfulMocks();

            // Override để quá 15 lịch hẹn
            _mockAppointmentService
                .Setup(s => s.CountAppointsByDate(It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(16);

            var availableSlots = new List<AppointmentDTO.AvailableAppointment>
            {
                new AppointmentDTO.AvailableAppointment { Date = DateTime.Now.AddDays(4), Time = "Sáng" },
                new AppointmentDTO.AvailableAppointment { Date = DateTime.Now.AddDays(4), Time = "Chiều" }
            };

            _mockAppointmentService
                .Setup(s => s.CheckAvailableAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(availableSlots);

            var form = MockData.ValidAppointmentForm;

            // Act
            var result = await _controller.Appointment(form) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            
            var response = result.Value;
            var availableProperty = response?.GetType().GetProperty("availableAppointments");
            Assert.That(availableProperty, Is.Not.Null, "Response phải chứa availableAppointments");
        }

        /// <summary>
        /// Test: Form null
        /// </summary>
        [Test]
        [Category("EdgeCase")]
        [Description("Form null")]
        public void Appointment_NullForm_ThrowsErrorHandlingException()
        {
            // Arrange
            AppointmentForm? form = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ErrorHandlingException>(async () => 
                await _controller.Appointment(form!));

            Assert.That(exception!.StatusCode, Is.EqualTo(400));
            Assert.That(exception.ErrorMessage, Is.EqualTo("Dữ liệu không hợp lệ"));
        }

        #endregion
    }
}