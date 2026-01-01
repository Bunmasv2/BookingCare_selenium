using ClosedXML.Excel;
using System.Text.Json;

namespace Server.Tests.Helpers
{
    /// <summary>
    /// Helper class để xuất báo cáo Unit Test WhiteBox cho chức năng Đặt lịch khám
    /// Dữ liệu trả về dạng JSON để kiểm tra API response
    /// </summary>
    public class AppointmentWhiteBoxTestReportHelper
    {
        private static List<WhiteBoxTestResult> _results = new List<WhiteBoxTestResult>();

        /// <summary>
        /// Cấu trúc kết quả test theo chuẩn WhiteBox Testing
        /// </summary>
        public class WhiteBoxTestResult
        {
            public string TestCaseId { get; set; } = "";
            public string MethodTested { get; set; } = "Appointment";
            public string Description { get; set; } = "";
            public string BranchCovered { get; set; } = "";
            public string CoverageType { get; set; } = ""; // Branch/Path/Condition
            public string PreCondition { get; set; } = "";
            public string InputData { get; set; } = "";
            public string ExpectedResult { get; set; } = "";
            public string ActualResult { get; set; } = "";
            public string Status { get; set; } = "";
            public string ExecutionTime { get; set; } = "";
        }

        /// <summary>
        /// Xóa tất cả kết quả test trước đó
        /// </summary>
        public static void ClearResults()
        {
            _results.Clear();
            LogToConsole("🧹 Đã xóa tất cả kết quả test cũ");
        }

        /// <summary>
        /// Ghi log ra terminal
        /// </summary>
        public static void LogToConsole(string message)
        {
            Console.WriteLine($"[AppointmentWhiteBox] {DateTime.Now:HH:mm:ss} - {message}");
        }

        /// <summary>
        /// Thêm kết quả test vào danh sách
        /// </summary>
        public static void AddTestResult(
            string testCaseId,
            string methodTested,
            string description,
            string branchCovered,
            string coverageType,
            string preCondition,
            object inputData,
            int expectedStatusCode,
            object expectedResponse,
            int actualStatusCode,
            object actualResponse,
            string verifyNeverCalled,
            bool testPassed,
            TimeSpan executionTime)
        {
            // JsonSerializer options với encoding hỗ trợ tiếng Việt (UTF-8)
            var jsonOptions = new JsonSerializerOptions 
            { 
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            // Serialize data to JSON
            var inputJson = JsonSerializer.Serialize(inputData, jsonOptions);
            var expectedJson = JsonSerializer.Serialize(expectedResponse, jsonOptions);
            var actualJson = JsonSerializer.Serialize(actualResponse, jsonOptions);

            // Kiểm tra pass/fail - status code phải khớp
            bool statusMatch = expectedStatusCode == actualStatusCode;
            
            // Response match: kiểm tra actual có chứa expected (cho phép thêm fields)
            bool responseMatch = actualJson.Contains(expectedJson.TrimStart('{').TrimEnd('}').Split(',')[0].Trim('"'));
            
            // Nếu expected có wildcard (*), chỉ cần kiểm tra phần đầu
            if (expectedJson.Contains("*"))
            {
                var expectedPrefix = expectedJson.Split('*')[0].Replace("\"", "").Replace("{", "").Replace("errorMessage:", "").Trim();
                responseMatch = actualJson.Contains(expectedPrefix);
            }
            
            // Final passed = test assertion passed VÀ status match
            bool finalPassed = testPassed && statusMatch;
            var status = finalPassed ? "PASS" : "FAIL";

            // Gom Expected và Actual thành object JSON
            var expectedResultJson = JsonSerializer.Serialize(new { statusCode = expectedStatusCode, response = expectedResponse }, jsonOptions);
            var actualResultJson = JsonSerializer.Serialize(new { statusCode = actualStatusCode, response = actualResponse }, jsonOptions);

            // Log chi tiết ra terminal
            LogToConsole($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            LogToConsole($"📋 Test Case: {testCaseId}");
            LogToConsole($"🔬 Method: {methodTested}");
            LogToConsole($"📝 Description: {description}");
            LogToConsole($"🌿 Branch Covered: {branchCovered}");
            LogToConsole($"📊 Coverage Type: {coverageType}");
            LogToConsole($"📥 Input: {inputJson.Substring(0, Math.Min(150, inputJson.Length))}...");
            LogToConsole($"🎯 Expected: {expectedResultJson}");
            LogToConsole($"📤 Actual: {actualResultJson}");
            LogToConsole($"⏱️ Execution Time: {executionTime.TotalMilliseconds:F2}ms");
            LogToConsole($"📊 Status Match: {(statusMatch ? "✅" : "❌")} | Response Match: {(responseMatch ? "✅" : "❌")}");
            LogToConsole($"🏆 Final Result: {(finalPassed ? "✅ PASS" : "❌ FAIL")}");
            LogToConsole($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            _results.Add(new WhiteBoxTestResult
            {
                TestCaseId = testCaseId,
                MethodTested = methodTested,
                Description = description,
                BranchCovered = branchCovered,
                CoverageType = coverageType,
                PreCondition = preCondition,
                InputData = inputJson,
                ExpectedResult = expectedResultJson,
                ActualResult = actualResultJson,
                Status = status,
                ExecutionTime = $"{executionTime.TotalMilliseconds:F2}ms"
            });
        }

        /// <summary>
        /// Xuất báo cáo ra file Excel
        /// </summary>
        public static void ExportToExcel(string filePath)
        {
            // Đảm bảo thư mục tồn tại
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("WhiteBox_Appointment");

                // Định nghĩa tiêu đề cột theo format WhiteBox Testing
                var headers = new string[]
                {
                    "Test Case ID",
                    "Method Tested",
                    "Description",
                    "Branch Covered",
                    "Coverage Type",
                    "PreCondition",
                    "Input Data (JSON)",
                    "Expected Result (JSON)",
                    "Actual Result (JSON)",
                    "Status",
                    "Execution Time"
                };

                // Style cho header
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565C0"); // Blue theme for Appointment
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                // Thêm dữ liệu
                for (int i = 0; i < _results.Count; i++)
                {
                    int row = i + 2;
                    var result = _results[i];

                    worksheet.Cell(row, 1).Value = result.TestCaseId;
                    worksheet.Cell(row, 2).Value = result.MethodTested;
                    worksheet.Cell(row, 3).Value = result.Description;
                    worksheet.Cell(row, 4).Value = result.BranchCovered;
                    worksheet.Cell(row, 5).Value = result.CoverageType;
                    worksheet.Cell(row, 6).Value = result.PreCondition;
                    worksheet.Cell(row, 7).Value = result.InputData;
                    worksheet.Cell(row, 8).Value = result.ExpectedResult;
                    worksheet.Cell(row, 9).Value = result.ActualResult;
                    worksheet.Cell(row, 10).Value = result.Status;
                    worksheet.Cell(row, 11).Value = result.ExecutionTime;

                    // Style cho Status
                    var statusCell = worksheet.Cell(row, 10);
                    if (result.Status == "PASS")
                    {
                        statusCell.Style.Font.FontColor = XLColor.Green;
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }
                    else
                    {
                        statusCell.Style.Font.FontColor = XLColor.Red;
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                    }

                    // Border cho tất cả cells
                    for (int j = 1; j <= headers.Length; j++)
                    {
                        worksheet.Cell(row, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(row, j).Style.Alignment.WrapText = true;
                        worksheet.Cell(row, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    }
                }

                // Điều chỉnh độ rộng cột
                worksheet.Column(1).Width = 12;   // Test Case ID
                worksheet.Column(2).Width = 25;   // Method Tested
                worksheet.Column(3).Width = 50;   // Description
                worksheet.Column(4).Width = 55;   // Branch Covered
                worksheet.Column(5).Width = 20;   // Coverage Type
                worksheet.Column(6).Width = 35;   // PreCondition
                worksheet.Column(7).Width = 50;   // Input Data JSON
                worksheet.Column(8).Width = 55;   // Expected Result JSON
                worksheet.Column(9).Width = 55;   // Actual Result JSON
                worksheet.Column(10).Width = 10;  // Status
                worksheet.Column(11).Width = 15;  // Execution Time

                // Freeze header row
                worksheet.SheetView.FreezeRows(1);

                // Xử lý trường hợp file đang mở
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    workbook.SaveAs(filePath);
                    Console.WriteLine($"✅ WhiteBox Appointment Report saved at: {filePath}");
                }
                catch (IOException)
                {
                    var newFilePath = Path.Combine(
                        Path.GetDirectoryName(filePath)!,
                        $"WhiteBox_Appointment_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                    );
                    workbook.SaveAs(newFilePath);
                    Console.WriteLine($"⚠️ File cũ đang mở, đã lưu báo cáo mới tại: {newFilePath}");
                }
            }
        }

        /// <summary>
        /// Lấy số lượng kết quả hiện tại
        /// </summary>
        public static int GetResultCount() => _results.Count;

        /// <summary>
        /// Lấy số test passed
        /// </summary>
        public static int GetPassedCount() => _results.Count(r => r.Status == "PASS");

        /// <summary>
        /// Lấy số test failed
        /// </summary>
        public static int GetFailedCount() => _results.Count(r => r.Status == "FAIL");
    }
}
