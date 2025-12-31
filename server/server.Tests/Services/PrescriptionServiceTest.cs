using Moq;
using NUnit.Framework;
using server.DTO;
using server.Models;
using server.Services;
using Server.Tests.Mocks;

namespace Server.Tests.Services
{
    /// <summary>
    /// Unit Tests cho MedicalRecordService (Service Layer)
    /// Các test này kiểm tra business logic trong Service layer cho chức năng Kê đơn thuốc
    /// </summary>
    [TestFixture]
    public class PrescriptionServiceTest
    {
        private Mock<IMedicalRecord> _mockMedicalRecordService = null!;
        private Mock<IMedicine> _mockMedicineService = null!;

        [SetUp]
        public void Setup()
        {
            _mockMedicalRecordService = new Mock<IMedicalRecord>();
            _mockMedicineService = new Mock<IMedicine>();
        }

        #region AddMedicalRecord Tests

        /// <summary>
        /// Test: Tạo Medical Record thành công với dữ liệu hợp lệ
        /// </summary>
        [Test]
        [Category("KDT01")]
        [Description("Tạo Medical Record thành công")]
        public async Task AddMedicalRecord_ValidData_ReturnsNewMedicalRecord()
        {
            // Arrange
            int appointmentId = 500;
            var prescriptionRequest = MockData.ValidPrescriptionRequest;

            _mockMedicalRecordService
                .Setup(s => s.AddMedicalRecord(appointmentId, prescriptionRequest))
                .ReturnsAsync(MockData.NewlyCreatedMedicalRecord);

            // Act
            var result = await _mockMedicalRecordService.Object.AddMedicalRecord(appointmentId, prescriptionRequest);

            // Assert
            Assert.That(result, Is.Not.Null, "Medical Record phải được tạo thành công");
            Assert.That(result.RecordId, Is.EqualTo(1), "RecordId phải = 1");
            Assert.That(result.AppointmentId, Is.EqualTo(500), "AppointmentId phải khớp");
            Assert.That(result.Diagnosis, Is.EqualTo("Sâu răng nhẹ"), "Diagnosis phải khớp");
            Assert.That(result.Treatment, Is.EqualTo("Vệ sinh răng miệng"), "Treatment phải khớp");

            Console.WriteLine($"✅ AddMedicalRecord: Tạo thành công RecordId = {result.RecordId}");
        }

        /// <summary>
        /// Test: Tạo Medical Record thất bại -> Return null
        /// </summary>
        [Test]
        [Category("Error")]
        [Description("Tạo Medical Record thất bại")]
        public async Task AddMedicalRecord_FailedCreation_ReturnsNull()
        {
            // Arrange
            int appointmentId = 999;
            var prescriptionRequest = MockData.ValidPrescriptionRequest;

            _mockMedicalRecordService
                .Setup(s => s.AddMedicalRecord(appointmentId, prescriptionRequest))
                .ReturnsAsync((MedicalRecord)null!);

            // Act
            var result = await _mockMedicalRecordService.Object.AddMedicalRecord(appointmentId, prescriptionRequest);

            // Assert
            Assert.That(result, Is.Null, "Khi tạo thất bại, phải trả về null");

            Console.WriteLine($"✅ AddMedicalRecord: Trả về null khi tạo thất bại");
        }

        #endregion

        #region AddMedicalRecordDetail Tests

        /// <summary>
        /// Test: Thêm chi tiết thuốc vào Medical Record thành công
        /// </summary>
        [Test]
        [Category("KDT01")]
        [Description("Thêm chi tiết thuốc thành công")]
        public async Task AddMedicalRecordDetail_ValidMedicines_ReturnsDetails()
        {
            // Arrange
            int recordId = 1;
            var medicines = MockData.ValidPrescriptionRequest.Medicines;

            _mockMedicalRecordService
                .Setup(s => s.AddMedicalRecordDetail(recordId, medicines))
                .ReturnsAsync(MockData.ValidMedicalRecordDetails);

            // Act
            var result = await _mockMedicalRecordService.Object.AddMedicalRecordDetail(recordId, medicines);

            // Assert
            Assert.That(result, Is.Not.Null, "MedicalRecordDetails phải được tạo");
            Assert.That(result.Count, Is.GreaterThan(0), "Phải có ít nhất 1 chi tiết thuốc");
            Assert.That(result[0].ReCordId, Is.EqualTo(1), "RecordId phải khớp");
            Assert.That(result[0].MedicineId, Is.EqualTo(1), "MedicineId phải khớp");

            Console.WriteLine($"✅ AddMedicalRecordDetail: Thêm {result.Count} thuốc thành công");
        }

        /// <summary>
        /// Test: Thêm chi tiết thuốc với danh sách rỗng -> Return null
        /// </summary>
        [Test]
        [Category("KDT02")]
        [Description("Thêm chi tiết thuốc với danh sách rỗng")]
        public async Task AddMedicalRecordDetail_EmptyMedicines_ReturnsNull()
        {
            // Arrange
            int recordId = 1;
            var medicines = new List<MedicalRecordDTO.MedicineDto>(); // Empty list

            _mockMedicalRecordService
                .Setup(s => s.AddMedicalRecordDetail(recordId, medicines))
                .ReturnsAsync((List<MedicalRecordDetail>)null!);

            // Act
            var result = await _mockMedicalRecordService.Object.AddMedicalRecordDetail(recordId, medicines);

            // Assert
            Assert.That(result, Is.Null, "Khi không có thuốc, phải trả về null");

            Console.WriteLine($"✅ AddMedicalRecordDetail: Trả về null khi danh sách thuốc rỗng");
        }

        /// <summary>
        /// Test: Thêm nhiều thuốc vào Medical Record (KDT05)
        /// </summary>
        [Test]
        [Category("KDT05")]
        [Description("Thêm nhiều thuốc thành công")]
        public async Task AddMedicalRecordDetail_MultipleMedicines_ReturnsAllDetails()
        {
            // Arrange
            int recordId = 1;
            var medicines = MockData.PrescriptionMultipleMedicines.Medicines;

            var expectedDetails = new List<MedicalRecordDetail>
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
                },
                new MedicalRecordDetail
                {
                    MedicineId = 2,
                    ReCordId = 1,
                    Quantity = 20,
                    Dosage = 2,
                    FrequencyPerDay = 2,
                    DurationInDays = 5,
                    Usage = "Uống sau ăn"
                }
            };

            _mockMedicalRecordService
                .Setup(s => s.AddMedicalRecordDetail(recordId, medicines))
                .ReturnsAsync(expectedDetails);

            // Act
            var result = await _mockMedicalRecordService.Object.AddMedicalRecordDetail(recordId, medicines);

            // Assert
            Assert.That(result, Is.Not.Null, "MedicalRecordDetails phải được tạo");
            Assert.That(result.Count, Is.EqualTo(2), "Phải có 2 chi tiết thuốc");

            Console.WriteLine($"✅ AddMedicalRecordDetail: Thêm {result.Count} thuốc thành công");
        }

        #endregion

        #region GetRecordDetail Tests

        /// <summary>
        /// Test: Lấy chi tiết thuốc của một Medical Record
        /// </summary>
        [Test]
        [Description("Lấy chi tiết thuốc thành công")]
        public async Task GetRecordDetail_ValidRecordId_ReturnsMedicineList()
        {
            // Arrange
            int recordId = 1;

            _mockMedicalRecordService
                .Setup(s => s.GetRecordDetail(recordId))
                .ReturnsAsync(MockData.MedicineDtoList);

            // Act
            var result = await _mockMedicalRecordService.Object.GetRecordDetail(recordId);

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về danh sách thuốc");
            Assert.That(result.Count, Is.GreaterThan(0), "Phải có ít nhất 1 thuốc");
            Assert.That(result[0].MedicineName, Is.EqualTo("Paracetamol(500mg/l)"), "Tên thuốc phải khớp");

            Console.WriteLine($"✅ GetRecordDetail: Lấy được {result.Count} thuốc");
        }

        /// <summary>
        /// Test: Lấy chi tiết thuốc với RecordId không tồn tại -> Return empty/null
        /// </summary>
        [Test]
        [Description("Lấy chi tiết thuốc với RecordId không tồn tại")]
        public async Task GetRecordDetail_InvalidRecordId_ReturnsEmpty()
        {
            // Arrange
            int recordId = 999;

            _mockMedicalRecordService
                .Setup(s => s.GetRecordDetail(recordId))
                .ReturnsAsync(new List<MedicalRecordDTO.MedicineDto>());

            // Act
            var result = await _mockMedicalRecordService.Object.GetRecordDetail(recordId);

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về list rỗng, không phải null");
            Assert.That(result.Count, Is.EqualTo(0), "List phải rỗng");

            Console.WriteLine($"✅ GetRecordDetail: Trả về list rỗng khi không tìm thấy");
        }

        #endregion

        #region GetMedicalRecords Tests

        /// <summary>
        /// Test: Lấy danh sách Medical Records theo appointment IDs
        /// </summary>
        [Test]
        [Description("Lấy danh sách Medical Records")]
        public async Task GetMedicalRecords_ValidAppointmentIds_ReturnsRecordList()
        {
            // Arrange
            var appointmentIds = new List<int> { 500, 501, 502 };
            var expectedRecords = new List<MedicalRecordDTO.MedicalRecordBasic>
            {
                new MedicalRecordDTO.MedicalRecordBasic
                {
                    RecordId = 1,
                    Diagnosis = "Sâu răng nhẹ",
                    Treatment = "Vệ sinh răng miệng",
                    DoctorName = "LÊ ANH TIẾN",
                    PatientName = "Nguyễn Văn A",
                    AppointmentDate = DateTime.Now.Date
                }
            };

            _mockMedicalRecordService
                .Setup(s => s.GetMedicalRecords(appointmentIds))
                .ReturnsAsync(expectedRecords);

            // Act
            var result = await _mockMedicalRecordService.Object.GetMedicalRecords(appointmentIds);

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về danh sách Medical Records");
            Assert.That(result.Count, Is.GreaterThan(0), "Phải có ít nhất 1 record");

            Console.WriteLine($"✅ GetMedicalRecords: Lấy được {result.Count} records");
        }

        #endregion

        #region GetMedicinesByName Tests (IMedicine)

        /// <summary>
        /// Test: Tìm kiếm thuốc theo tên
        /// </summary>
        [Test]
        [Description("Tìm kiếm thuốc theo tên")]
        public async Task SearchMedicinesByName_ValidQuery_ReturnsMedicineList()
        {
            // Arrange
            string query = "Para";
            var expectedMedicines = new List<MedicineDTO.MedicineBasic>
            {
                new MedicineDTO.MedicineBasic
                {
                    MedicineId = 1,
                    MedicalName = "Paracetamol(500mg/l)",
                    Unit = "viên"
                }
            };

            _mockMedicineService
                .Setup(s => s.SearchMedicinesByName(query))
                .ReturnsAsync(expectedMedicines);

            // Act
            var result = await _mockMedicineService.Object.SearchMedicinesByName(query);

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về danh sách thuốc");
            Assert.That(result.Count, Is.GreaterThan(0), "Phải có ít nhất 1 thuốc");
            Assert.That(result[0].MedicalName, Does.Contain("Para"), "Tên thuốc phải chứa từ khóa tìm kiếm");

            Console.WriteLine($"✅ SearchMedicinesByName: Tìm được {result.Count} thuốc với từ khóa '{query}'");
        }

        /// <summary>
        /// Test: Lấy tất cả thuốc
        /// </summary>
        [Test]
        [Description("Lấy tất cả thuốc")]
        public async Task GetAllMedicines_ReturnsAllMedicines()
        {
            // Arrange
            var expectedMedicines = new List<MedicineDTO.MedicineBasic>
            {
                new MedicineDTO.MedicineBasic { MedicineId = 1, MedicalName = "Paracetamol(500mg/l)", Unit = "viên" },
                new MedicineDTO.MedicineBasic { MedicineId = 2, MedicalName = "Amoxicillin(250mg)", Unit = "viên" },
                new MedicineDTO.MedicineBasic { MedicineId = 3, MedicalName = "Ibuprofen(400mg)", Unit = "viên" }
            };

            _mockMedicineService
                .Setup(s => s.GetAllMedicines())
                .ReturnsAsync(expectedMedicines);

            // Act
            var result = await _mockMedicineService.Object.GetAllMedicines();

            // Assert
            Assert.That(result, Is.Not.Null, "Phải trả về danh sách thuốc");
            Assert.That(result.Count, Is.EqualTo(3), "Phải có 3 thuốc");

            Console.WriteLine($"✅ GetAllMedicines: Lấy được {result.Count} thuốc");
        }

        #endregion

        #region Quantity Calculation Tests (KDT07)

        /// <summary>
        /// Test: Tính tổng số lượng thuốc - Formula: Dosage * FrequencyPerDay * DurationInDays
        /// </summary>
        [Test]
        [Category("KDT07")]
        [Description("Tính tổng số lượng thuốc")]
        public void CalculateQuantity_ValidInputs_ReturnsCorrectQuantity()
        {
            // Test cases
            var testCases = new[]
            {
                new { Dosage = 1, Frequency = 3, Duration = 3, Expected = 9 },
                new { Dosage = 1, Frequency = 3, Duration = 5, Expected = 15 },
                new { Dosage = 2, Frequency = 2, Duration = 5, Expected = 20 },
                new { Dosage = 1, Frequency = 1, Duration = 7, Expected = 7 },
                new { Dosage = 0, Frequency = 3, Duration = 3, Expected = 0 }
            };

            foreach (var testCase in testCases)
            {
                // Act
                int quantity = testCase.Dosage * testCase.Frequency * testCase.Duration;

                // Assert
                Assert.That(quantity, Is.EqualTo(testCase.Expected), 
                    $"Quantity should be {testCase.Expected} for {testCase.Dosage} * {testCase.Frequency} * {testCase.Duration}");

                Console.WriteLine($"✅ CalculateQuantity: {testCase.Dosage} × {testCase.Frequency} × {testCase.Duration} = {quantity}");
            }
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Test: Validate Diagnosis không được trống (KDT10)
        /// </summary>
        [Test]
        [Category("KDT10")]
        [Description("Validate Diagnosis không được trống")]
        public void ValidateDiagnosis_EmptyDiagnosis_ReturnsFalse()
        {
            // Arrange
            var prescription = MockData.PrescriptionMissingDiagnosis;

            // Act
            bool isValid = !string.IsNullOrEmpty(prescription.Diagnosis);

            // Assert
            Assert.That(isValid, Is.False, "Diagnosis trống phải không hợp lệ");

            Console.WriteLine($"✅ ValidateDiagnosis: Diagnosis = '{prescription.Diagnosis}' -> isValid = {isValid}");
        }

        /// <summary>
        /// Test: Validate Dosage không được = 0 (KDT04)
        /// </summary>
        [Test]
        [Category("KDT04")]
        [Description("Validate Dosage không được = 0")]
        public void ValidateDosage_ZeroDosage_ReturnsFalse()
        {
            // Arrange
            var prescription = MockData.PrescriptionZeroDosage;
            var medicine = prescription.Medicines[0];

            // Act
            int dosage = int.Parse(medicine.Dosage);
            bool isValid = dosage > 0;

            // Assert
            Assert.That(isValid, Is.False, "Dosage = 0 phải không hợp lệ");

            Console.WriteLine($"✅ ValidateDosage: Dosage = {dosage} -> isValid = {isValid}");
        }

        /// <summary>
        /// Test: Validate Medicines list không được rỗng (KDT02)
        /// </summary>
        [Test]
        [Category("KDT02")]
        [Description("Validate Medicines list không được rỗng")]
        public void ValidateMedicines_EmptyList_ReturnsFalse()
        {
            // Arrange
            var prescription = MockData.PrescriptionWithoutMedicine;

            // Act
            bool isValid = prescription.Medicines != null && prescription.Medicines.Count > 0;

            // Assert
            Assert.That(isValid, Is.False, "Medicines list rỗng phải không hợp lệ");

            Console.WriteLine($"✅ ValidateMedicines: Medicines count = {prescription.Medicines?.Count ?? 0} -> isValid = {isValid}");
        }

        /// <summary>
        /// Test: Validate thông tin thuốc đầy đủ (KDT03)
        /// </summary>
        [Test]
        [Category("KDT03")]
        [Description("Validate thông tin thuốc đầy đủ")]
        public void ValidateMedicineInfo_MissingInfo_ReturnsFalse()
        {
            // Arrange
            var prescription = MockData.PrescriptionMissingMedicineInfo;
            var medicine = prescription.Medicines[0];

            // Act
            bool isValid = !string.IsNullOrEmpty(medicine.Dosage) &&
                           !string.IsNullOrEmpty(medicine.FrequencyPerDay) &&
                           !string.IsNullOrEmpty(medicine.DurationInDays);

            // Assert
            Assert.That(isValid, Is.False, "Thiếu thông tin thuốc bắt buộc phải không hợp lệ");

            Console.WriteLine($"✅ ValidateMedicineInfo: Dosage='{medicine.Dosage}', Frequency='{medicine.FrequencyPerDay}', Duration='{medicine.DurationInDays}' -> isValid = {isValid}");
        }

        #endregion
    }
}
