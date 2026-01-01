using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Diagnostics;
using server.Controllers;
using server.Services;
using server.Models;
using server.DTO;
using server.Middleware;
using Microsoft.Extensions.Configuration;
using Server.Tests.Mocks;
using Server.Tests.Helpers;
using Server.DTO;
using AppointmentModel = server.Models.Appointment;

namespace Server.Tests.Controllers.AppointmentTests
{
    /// <summary>
    /// Unit Tests WhiteBox cho AppointmentsController
    /// Bám sát logic từng dòng if/else, throw exception trong controller
    /// 
    /// Coverage Types:
    /// - Branch Coverage: Phủ nhánh - kiểm tra từng nhánh if/else
    /// - Path Coverage: Phủ đường - kiểm tra các đường đi trong code
    /// - Condition Coverage: Phủ điều kiện - kiểm tra từng điều kiện trong biểu thức logic
    /// </summary>
    [TestFixture]
    public class AppointmentWhiteBoxTests
    {
        #region Private Fields

        private Mock<ClinicManagementContext> _mockContext = null!;
        private Mock<IDoctor> _mockDoctorService = null!;
        private Mock<IPatient> _mockPatientService = null!;
        private Mock<IAppointment> _mockAppointmentService = null!;
        private Mock<ISpecialty> _mockSpecialtyService = null!;
        private Mock<IService> _mockServiceService = null!;
        private Mock<IConfiguration> _mockConfig = null!;
        private AppointmentsController _controller = null!;
        private Stopwatch _stopwatch = null!;

        #endregion

        #region Setup & Teardown

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AppointmentWhiteBoxTestReportHelper.ClearResults();
            Console.WriteLine("\n");
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     📅 WHITEBOX UNIT TEST - AppointmentsController                       ║");
            Console.WriteLine("║     Framework: NUnit 4.4.0 + Moq 4.20.72                                 ║");
            Console.WriteLine("║     Coverage: Branch + Path + Condition Coverage                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝\n");
        }

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<ClinicManagementContext>();
            _mockDoctorService = new Mock<IDoctor>();
            _mockPatientService = new Mock<IPatient>();
            _mockAppointmentService = new Mock<IAppointment>();
            _mockSpecialtyService = new Mock<ISpecialty>();
            _mockServiceService = new Mock<IService>();
            _mockConfig = new Mock<IConfiguration>();

            _controller = new AppointmentsController(
                null!,
                _mockDoctorService.Object,
                _mockPatientService.Object,
                _mockAppointmentService.Object,
                _mockSpecialtyService.Object,
                _mockServiceService.Object,
                _mockConfig.Object
            );

            _stopwatch = new Stopwatch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var reportPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "Report", "WhiteBox_Appointment_Report.xlsx"
            );
            reportPath = Path.GetFullPath(reportPath);

            Console.WriteLine("\n");
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📊 KẾT QUẢ TỔNG HỢP WHITEBOX - APPOINTMENT            ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  📋 Tổng số Test Cases: {AppointmentWhiteBoxTestReportHelper.GetResultCount(),-50} ║");
            Console.WriteLine($"║  ✅ Passed: {AppointmentWhiteBoxTestReportHelper.GetPassedCount(),-62} ║");
            Console.WriteLine($"║  ❌ Failed: {AppointmentWhiteBoxTestReportHelper.GetFailedCount(),-62} ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════╣");

            AppointmentWhiteBoxTestReportHelper.ExportToExcel(reportPath);

            Console.WriteLine($"║  📁 Report: {reportPath.Substring(Math.Max(0, reportPath.Length - 60)),-62} ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝\n");
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

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

        private void SetupUserContextWithRole(int userId, string role)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["role"] = role;
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
            }, "mock"));

            httpContext.User = user;

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        #endregion

        #region DLK01 - Form null - Branch Coverage

        /// <summary>
        /// Test Case DLK01: Kiểm tra khi AppointmentForm null
        /// Branch: Line 49 - if (appointmentForm == null) -> throw ErrorHandlingException(400)
        /// Coverage Type: BRANCH COVERAGE - Phủ nhánh if null
        /// </summary>
        [Test, Order(1)]
        [Category("WhiteBox")]
        [Category("DLK01")]
        [Category("BranchCoverage")]
        public void DLK01_AppointmentFormNull_ThrowsErrorHandlingException()
        {
            // Test metadata
            string testCaseId = "DLK01";
            string methodTested = "Appointment";
            string description = "Kiểm tra khi bệnh nhân không điền form và gửi form";
            string branchCovered = "Line 49: if (appointmentForm == null) -> throw ErrorHandlingException(400)";
            string coverageType = "Branch Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, gửi form null";

            int expectedStatusCode = 400;
            var expectedResponse = new { errorMessage = "Dữ liệu sai!" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");
                AppointmentForm? nullForm = null;

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(nullForm!));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify - Không có service nào được gọi
                _mockSpecialtyService.Verify(
                    s => s.GetSpecialty(It.IsAny<string>()),
                    Times.Never,
                    "GetSpecialty KHÔNG được gọi khi form null");

                _mockDoctorService.Verify(
                    d => d.GetDoctorByName(It.IsAny<string>()),
                    Times.Never,
                    "GetDoctorByName KHÔNG được gọi khi form null");

                verifyNeverCalled = "GetSpecialty, GetDoctorByName, GetPatientByUserId";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { form = "null" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK02 - Department Empty - Branch Coverage

        /// <summary>
        /// Test Case DLK02: Kiểm tra khi Department rỗng
        /// Branch: Line 52 - if (string.IsNullOrWhiteSpace(appointmentForm.Department)) -> throw
        /// Coverage Type: BRANCH COVERAGE - Phủ nhánh kiểm tra Department
        /// 
        /// ⚠️ FAIL CASE: Expected StatusCode = 404 (SAI) nhưng Actual = 400 (ĐÚNG)
        /// </summary>
        [Test, Order(2)]
        [Category("WhiteBox")]
        [Category("DLK02")]
        [Category("BranchCoverage")]
        [Category("FailCase")]
        public void DLK02_DepartmentEmpty_ThrowsErrorHandlingException_FAIL()
        {
            // Test metadata
            string testCaseId = "DLK02";
            string methodTested = "Appointment";
            string description = "Kiểm tra khi bệnh nhân không chọn chuyên khoa";
            string branchCovered = "Line 52: if (string.IsNullOrWhiteSpace(appointmentForm.Department)) -> throw";
            string coverageType = "Branch Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, form thiếu Department";

            // ⚠️ CỐ TÌNH ĐẶT SAI: Expected = 404, nhưng controller throw 400
            int expectedStatusCode = 404; // SAI - Thực tế là 400
            var expectedResponse = new { errorMessage = "Vui lòng chọn khoa" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");
                var form = MockData.FormMissingDepartment;

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(form));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify - Không gọi service validation tiếp theo
                _mockSpecialtyService.Verify(
                    s => s.GetSpecialty(It.IsAny<string>()),
                    Times.Never,
                    "GetSpecialty KHÔNG được gọi khi Department rỗng");

                verifyNeverCalled = "GetSpecialty, GetDoctorByName";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { department = "", doctor = form.Doctor, service = form.Service },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }
        
        // Khai báo form ngoài try block để dùng trong finally
        private AppointmentForm form = MockData.FormMissingDepartment;

        #endregion

        #region DLK03 - Symptoms Too Long - Condition Coverage

        /// <summary>
        /// Test Case DLK03: Kiểm tra điều kiện Symptoms quá dài
        /// Branch: Line 67 - if (!string.IsNullOrWhiteSpace(appointmentForm.Symptoms) && appointmentForm.Symptoms.Count() > 500)
        /// Coverage Type: CONDITION COVERAGE - Phủ điều kiện kép (NOT empty AND > 500)
        /// </summary>
        [Test, Order(3)]
        [Category("WhiteBox")]
        [Category("DLK03")]
        [Category("ConditionCoverage")]
        public void DLK03_SymptomsTooLong_ThrowsErrorHandlingException()
        {
            // Test metadata
            string testCaseId = "DLK03";
            string methodTested = "Appointment";
            string description = "Kiểm tra điều kiện triệu chứng quá dài (>500 ký tự)";
            string branchCovered = "Line 67: if (!IsNullOrWhiteSpace(Symptoms) && Symptoms.Count() > 500) -> throw";
            string coverageType = "Condition Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, Symptoms có 501 ký tự";

            int expectedStatusCode = 400;
            var expectedResponse = new { errorMessage = "Triệu chứng quá dài" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;
            var testForm = MockData.FormSymptomsTooLong;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");
                
                // Đảm bảo symptoms có đúng 501 ký tự
                Assert.That(testForm.Symptoms.Length, Is.EqualTo(501), "Symptoms phải có 501 ký tự");

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(testForm));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify điều kiện: cả 2 phần của AND đều TRUE
                // !IsNullOrWhiteSpace = TRUE (vì có nội dung)
                // Count() > 500 = TRUE (vì có 501 ký tự)
                verifyNeverCalled = "GetSpecialty (validation thất bại trước)";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { symptomsLength = testForm.Symptoms.Length, condition1 = "NOT empty = TRUE", condition2 = ">500 = TRUE" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK04 - Doctor Not Found After Validation - Path Coverage

        /// <summary>
        /// Test Case DLK04: Kiểm tra đường đi khi không tìm thấy bác sĩ
        /// Branch: Line 77 - if (doctor == null) -> throw ErrorHandlingException(404)
        /// Coverage Type: PATH COVERAGE - Đường đi khi Doctor không tồn tại
        /// </summary>
        [Test, Order(4)]
        [Category("WhiteBox")]
        [Category("DLK04")]
        [Category("PathCoverage")]
        public void DLK04_DoctorNotFound_AfterValidationAndSpecialtyCheck_ThrowsException()
        {
            // Test metadata
            string testCaseId = "DLK04";
            string methodTested = "Appointment";
            string description = "Kiểm tra khi không tìm thấy bác sĩ sau khi đã validate form và chuyên khoa";
            string branchCovered = "Line 77: if (doctor == null) -> throw ErrorHandlingException(404, 'Không tìm thấy bác sĩ')";
            string coverageType = "Path Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, form hợp lệ, Specialty tồn tại, Doctor KHÔNG tồn tại";

            int expectedStatusCode = 404;
            var expectedResponse = new { errorMessage = "Bác sĩ không tồn tại" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var testForm = MockData.ValidAppointmentForm;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");
                
                _mockSpecialtyService
                    .Setup(s => s.GetSpecialty(testForm.Department))
                    .ReturnsAsync(new Specialty 
                    { 
                        SpecialtyId = 1, 
                        Name = testForm.Department 
                    });

                _mockDoctorService
                    .Setup(d => d.GetDoctorByName(testForm.Doctor))
                    .ReturnsAsync((DoctorDTO.DoctorDetail?)null);

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(testForm));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify - GetPatientByUserId KHÔNG được gọi vì Doctor null
                _mockPatientService.Verify(
                    p => p.GetPatientByUserId(It.IsAny<int>()),
                    Times.Never,
                    "GetPatientByUserId KHÔNG được gọi khi Doctor null");

                _mockServiceService.Verify(
                    s => s.GetServiceByName(It.IsAny<string>()),
                    Times.Never,
                    "GetServiceByName KHÔNG được gọi khi Doctor null");

                _mockAppointmentService.Verify(
                    a => a.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()),
                    Times.Never,
                    "IsExistAppointment KHÔNG được gọi khi Doctor null");

                verifyNeverCalled = "GetPatientByUserId, GetServiceByName, IsExistAppointment, Appointment";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { department = testForm.Department, doctor = testForm.Doctor },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK05 - Date Too Close (diffDays < 1) - Branch & Condition Coverage

        /// <summary>
        /// Test Case DLK05: Kiểm tra ngày đặt lịch quá gần (trong vòng 1 ngày)
        /// Branch: Line 91 - if (diffDays < 1) -> throw ErrorHandlingException(400)
        /// Coverage Type: BRANCH & CONDITION COVERAGE - Kiểm tra điều kiện diffDays < 1
        /// </summary>
        [Test, Order(5)]
        [Category("WhiteBox")]
        [Category("DLK05")]
        [Category("BranchConditionCoverage")]
        public void DLK05_DateTooClose_ThrowsErrorHandlingException()
        {
            // Test metadata
            string testCaseId = "DLK05";
            string methodTested = "Appointment";
            string description = "Kiểm tra ngày đặt lịch trong vòng 1 ngày (diffDays < 1)";
            string branchCovered = "Line 91: if (diffDays < 1) -> throw ErrorHandlingException(400, 'Vui lòng đặt lịch tối thiểu trước 1 ngày')";
            string coverageType = "Branch & Condition Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, form hợp lệ, ngày đặt = hôm nay";

            int expectedStatusCode = 400;
            var expectedResponse = new { errorMessage = "Vui lòng đặt lịch khám tối thiểu trước 1 ngày" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            // Form với ngày hôm nay (diffDays = 0)
            var testForm = new AppointmentForm
            {
                Department = "Khoa Răng - Hàm - Mặt",
                Doctor = "LÊ ANH TIẾN",
                Service = "Tẩy trắng răng",
                AppointmentDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), // Hôm nay
                AppointmentTime = "Sáng",
                Symptoms = "Đau răng"
            };

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");

                // Mock tất cả services trả về hợp lệ để đi đến bước check date
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
                    .ReturnsAsync(MockData.ValidService);

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(testForm));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify - IsExistAppointment KHÔNG được gọi vì date validation fail trước
                _mockAppointmentService.Verify(
                    a => a.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()),
                    Times.Never,
                    "IsExistAppointment KHÔNG được gọi khi diffDays < 1");

                verifyNeverCalled = "IsExistAppointment, CountAppointsByDate, Appointment";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { appointmentDate = testForm.AppointmentDate, diffDays = 0, condition = "diffDays < 1 = TRUE" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK06 - Date Too Far (diffDays > 15) - Branch Coverage FAIL CASE

        /// <summary>
        /// Test Case DLK06: Kiểm tra ngày đặt lịch quá xa (>15 ngày)
        /// Branch: Line 97 - if (diffDays > 15) -> throw ErrorHandlingException(400)
        /// Coverage Type: BRANCH COVERAGE
        /// 
        /// ⚠️ FAIL CASE: Expected StatusCode = 500 (SAI) nhưng Actual = 400 (ĐÚNG)
        /// </summary>
        [Test, Order(6)]
        [Category("WhiteBox")]
        [Category("DLK06")]
        [Category("BranchCoverage")]
        [Category("FailCase")]
        public void DLK06_DateTooFar_ThrowsErrorHandlingException_FAIL()
        {
            // Test metadata
            string testCaseId = "DLK06";
            string methodTested = "Appointment";
            string description = "Kiểm tra ngày đặt lịch quá xa (diffDays > 15)E";
            string branchCovered = "Line 97: if (diffDays > 15) -> throw ErrorHandlingException(400)";
            string coverageType = "Branch Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, ngày đặt > 15 ngày so với hôm nay";

            // ⚠️ CỐ TÌNH ĐẶT SAI: Expected = 500, nhưng controller throw 400
            int expectedStatusCode = 500; // SAI - Thực tế là 400
            var expectedResponse = new { errorMessage = "Ngày khám không được cách quá 15 ngày so với hôm nay" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var testForm = MockData.FormDateTooFar; // Ngày + 20 ngày

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");

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
                    .ReturnsAsync(MockData.ValidService);

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(testForm));

                Assert.That(ex, Is.Not.Null);
                
                // Actual là 400 - Controller throw đúng
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // So sánh với expected (500) -> FAIL vì 400 != 500
                testPassed = (actualStatusCode == expectedStatusCode);

                verifyNeverCalled = "IsExistAppointment, Appointment";
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { appointmentDate = testForm.AppointmentDate, diffDays = 20, condition = "diffDays > 15 = TRUE" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK07 - Existing Appointment Not Completed - Path Coverage

        /// <summary>
        /// Test Case DLK07: Kiểm tra khi đã có lịch hẹn chưa hoàn thành
        /// Branch: Line 104-112 - if (isExistAppointment != null) -> throw ErrorHandlingException(400)
        /// Coverage Type: PATH COVERAGE - Đường đi khi có appointment trùng
        /// </summary>
        [Test, Order(7)]
        [Category("WhiteBox")]
        [Category("DLK07")]
        [Category("PathCoverage")]
        public void DLK07_ExistingAppointmentNotCompleted_ThrowsErrorHandlingException()
        {
            // Test metadata
            string testCaseId = "DLK07";
            string methodTested = "Appointment";
            string description = "Kiểm tra khi có lịch hẹn chưa hoàn thành";
            string branchCovered = "Line 104-112: if (isExistAppointment != null) -> throw ErrorHandlingException(400)";
            string coverageType = "Path Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, đã có lịch hẹn chưa hoàn thành";

            int expectedStatusCode = 400;
            var expectedResponse = new { errorMessage = "Bạn chưa hoàn thành lịch hẹn *" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var testForm = MockData.ValidAppointmentForm;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");

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
                    .ReturnsAsync(MockData.ValidService);

                // Mock có lịch hẹn chưa hoàn thành
                _mockAppointmentService
                    .Setup(a => a.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(MockData.ExistingUnfinishedAppointment);

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.Appointment(testForm));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify path - CountAppointsByDate KHÔNG được gọi
                _mockAppointmentService.Verify(
                    a => a.CountAppointsByDate(It.IsAny<DateTime>(), It.IsAny<string>()),
                    Times.Never,
                    "CountAppointsByDate KHÔNG được gọi khi có appointment chưa hoàn thành");

                verifyNeverCalled = "CountAppointsByDate, Appointment";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { existingAppointmentId = 888, existingStatus = "Chờ xác nhận" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK08 - Slot Full (quantityAppointment > 15) - Branch Coverage

        /// <summary>
        /// Test Case DLK08: Kiểm tra khi slot đã đầy (>15 lịch hẹn)
        /// Branch: Line 119-128 - if (quantityAppointment > 15) -> return availableAppointments
        /// Coverage Type: BRANCH COVERAGE - Nhánh slot đầy
        /// </summary>
        [Test, Order(8)]
        [Category("WhiteBox")]
        [Category("DLK08")]
        [Category("BranchCoverage")]
        public async Task DLK08_SlotFull_ReturnsAvailableAppointments()
        {
            // Test metadata
            string testCaseId = "DLK08";
            string methodTested = "Appointment";
            string description = "Kiểm tra khi slot đã đầy (>15 lịch hẹn)";
            string branchCovered = "Line 119-128: if (quantityAppointment > 15) -> return Ok(availableAppointments)";
            string coverageType = "Branch Coverage";
            string preCondition = "Bệnh nhân đã đăng nhập, slot đã có 16+ lịch hẹn";

            int expectedStatusCode = 200;
            var expectedResponse = new { availableAppointments = "*" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var testForm = MockData.ValidAppointmentForm;
            var availableSlots = new List<AppointmentDTO.AvailableAppointment>
            {
                new AppointmentDTO.AvailableAppointment { Date = DateTime.Now.AddDays(4), Time = "Chiều" },
                new AppointmentDTO.AvailableAppointment { Date = DateTime.Now.AddDays(5), Time = "Sáng" }
            };

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");

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
                    .ReturnsAsync(MockData.ValidService);

                // Mock không có appointment trước đó
                _mockAppointmentService
                    .Setup(a => a.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync((AppointmentModel?)null);

                // Mock slot đầy (16 lịch hẹn)
                _mockAppointmentService
                    .Setup(a => a.CountAppointsByDate(It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(16);

                // Mock trả về các slot khả dụng
                _mockAppointmentService
                    .Setup(a => a.CheckAvailableAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(availableSlots);

                // Act
                var result = await _controller.Appointment(testForm);

                // Assert
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                actualStatusCode = okResult!.StatusCode ?? 200;
                actualResponse = okResult.Value!;

                // Verify - Appointment service KHÔNG được gọi (vì slot đầy)
                _mockAppointmentService.Verify(
                    a => a.Appointment(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), 
                        It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Never,
                    "Appointment KHÔNG được tạo khi slot đầy");

                // Verify CheckAvailableAppointment được gọi
                _mockAppointmentService.Verify(
                    a => a.CheckAvailableAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()),
                    Times.Once,
                    "CheckAvailableAppointment phải được gọi đúng 1 lần");

                verifyNeverCalled = "Appointment (service method)";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { slotCount = 16, condition = "quantityAppointment > 15 = TRUE" },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK09 - Successful Appointment - Full Path Coverage

        /// <summary>
        /// Test Case DLK09: Đặt lịch thành công - Full Path
        /// Branch: Line 130-139 - Tất cả validation pass -> Appointment created
        /// Coverage Type: PATH COVERAGE - Đường đi thành công đầy đủ
        /// </summary>
        [Test, Order(9)]
        [Category("WhiteBox")]
        [Category("DLK09")]
        [Category("PathCoverage")]
        public async Task DLK09_SuccessfulAppointment_ReturnsOkResult()
        {
            // Test metadata
            string testCaseId = "DLK09";
            string methodTested = "Appointment";
            string description = "Đặt lịch thành công";
            string branchCovered = "Line 130-139: All validations pass -> Appointment() -> return Ok('Đặt lịch thành công!')";
            string coverageType = "Path Coverage (Happy Path)";
            string preCondition = "Bệnh nhân đã đăng nhập, tất cả dữ liệu hợp lệ, slot còn trống";

            int expectedStatusCode = 200;
            var expectedResponse = new { message = "Đặt lịch thành công!" };

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var testForm = MockData.ValidAppointmentForm;

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContext(MockData.PatientUserId, "patient");

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
                    .ReturnsAsync(MockData.ValidService);

                // Mock không có appointment trước đó
                _mockAppointmentService
                    .Setup(a => a.IsExistAppointment(It.IsAny<int?>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync((AppointmentModel?)null);

                // Mock slot còn trống (10 lịch hẹn < 15)
                _mockAppointmentService
                    .Setup(a => a.CountAppointsByDate(It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(10);

                // Mock tạo appointment thành công
                _mockAppointmentService
                    .Setup(a => a.Appointment(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                        It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(MockData.NewlyCreatedAppointment);

                // Act
                var result = await _controller.Appointment(testForm);

                // Assert
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                actualStatusCode = okResult!.StatusCode ?? 200;
                actualResponse = okResult.Value!;

                // Verify tất cả services được gọi đúng thứ tự
                _mockSpecialtyService.Verify(s => s.GetSpecialty(testForm.Department), Times.Once);
                _mockDoctorService.Verify(d => d.GetDoctorByName(testForm.Doctor), Times.Once);
                _mockPatientService.Verify(p => p.GetPatientByUserId(MockData.PatientUserId), Times.Once);
                _mockServiceService.Verify(s => s.GetServiceByName(testForm.Service), Times.Once);
                _mockAppointmentService.Verify(a => a.Appointment(
                    MockData.ValidPatient.PatientId,
                    MockData.ValidDoctor.DoctorId,
                    MockData.ValidService.ServiceId,
                    It.IsAny<DateTime>(),
                    testForm.AppointmentTime,
                    "Chờ xác nhận"
                ), Times.Once);

                verifyNeverCalled = "N/A - Full path executed";
                testPassed = (actualStatusCode == expectedStatusCode);
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { 
                        department = testForm.Department, 
                        doctor = testForm.Doctor,
                        service = testForm.Service,
                        date = testForm.AppointmentDate,
                        time = testForm.AppointmentTime 
                    },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion

        #region DLK10 - UpdateStatus Doctor Role Check - Branch & Condition Coverage FAIL CASE

        /// <summary>
        /// Test Case DLK10: Kiểm tra quyền bác sĩ cập nhật status
        /// Branch: Line 185-189 - if (role == "doctor" && statusUpdate.Status != "Đã khám") -> throw 403
        /// Coverage Type: BRANCH & CONDITION COVERAGE
        /// 
        /// ⚠️ FAIL CASE: Expected error message sai
        /// </summary>
        [Test, Order(10)]
        [Category("WhiteBox")]
        [Category("DLK10")]
        [Category("BranchConditionCoverage")]
        [Category("FailCase")]
        public void DLK10_DoctorUpdateStatusNotDaKham_ThrowsForbidden_FAIL()
        {
            // Test metadata
            string testCaseId = "DLK10";
            string methodTested = "UpdateAppointmentStatus";
            string description = "Kiểm tra bác sĩ không có quyền cập nhật status khác 'Đã khám'";
            string branchCovered = "Line 185-189: if (role == 'doctor' && status != 'Đã khám') -> throw 403";
            string coverageType = "Branch & Condition Coverage";
            string preCondition = "Bác sĩ đã đăng nhập, cố cập nhật status thành 'Đã xác nhận'";

            // ⚠️ CỐ TÌNH ĐẶT SAI: Expected message sai
            int expectedStatusCode = 403;
            var expectedResponse = new { errorMessage = "Không có quyền truy cập!" }; // SAI - Thực tế là "Bạn không có quyền!"

            int actualStatusCode = 0;
            object actualResponse = new { };
            string verifyNeverCalled = "";
            bool testPassed = false;

            var statusUpdate = new UpdateStatusDTO { Status = "Đã xác nhận" };

            _stopwatch.Restart();

            try
            {
                // Arrange
                SetupUserContextWithRole(MockData.DoctorUserId, "doctor");

                // Act & Assert
                var ex = Assert.ThrowsAsync<ErrorHandlingException>(async () =>
                    await _controller.UpdateAppointmentStatus(1, statusUpdate));

                Assert.That(ex, Is.Not.Null);
                actualStatusCode = ex!.StatusCode;
                actualResponse = new { errorMessage = ex.ErrorMessage };

                // Verify - GetAppointmentById KHÔNG được gọi vì authorization fail trước
                _mockAppointmentService.Verify(
                    a => a.GetAppointmentById(It.IsAny<int>()),
                    Times.Never,
                    "GetAppointmentById KHÔNG được gọi khi authorization fail");

                verifyNeverCalled = "GetAppointmentById, UpdateStatus";
                
                // So sánh với expected -> FAIL vì message khác
                testPassed = (actualStatusCode == expectedStatusCode) && 
                             (ex.ErrorMessage == "Không có quyền truy cập!");
            }
            catch (Exception ex) when (ex is not AssertionException)
            {
                actualResponse = new { error = ex.Message };
            }
            finally
            {
                _stopwatch.Stop();
                AppointmentWhiteBoxTestReportHelper.AddTestResult(
                    testCaseId,
                    methodTested,
                    description,
                    branchCovered,
                    coverageType,
                    preCondition,
                    new { 
                        role = "doctor", 
                        newStatus = "Đã xác nhận",
                        condition1 = "role == 'doctor' = TRUE",
                        condition2 = "status != 'Đã khám' = TRUE"
                    },
                    expectedStatusCode,
                    expectedResponse,
                    actualStatusCode,
                    actualResponse,
                    verifyNeverCalled,
                    testPassed,
                    _stopwatch.Elapsed
                );
            }
        }

        #endregion
    }
}
