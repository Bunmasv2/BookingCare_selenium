using ClosedXML.Excel;

namespace Server.Tests.Helpers
{
    /// <summary>
    /// Helper class để xuất báo cáo Unit Test cho chức năng Đặt lịch khám
    /// Tham khảo cấu trúc từ TestReportHelper.cs
    /// </summary>
    public class AppointmentTestReportHelper
    {
        private static List<AppointmentTestResult> _results = new List<AppointmentTestResult>();

        /// <summary>
        /// Cấu trúc kết quả test theo format Excel
        /// </summary>
        public class AppointmentTestResult
        {
            public string Id { get; set; } = "";
            public string Items { get; set; } = "Đặt lịch khám";
            public string Description { get; set; } = "";
            public string PreCondition { get; set; } = "";
            public string StepsToExecute { get; set; } = "";
            public string ExpectedOutput { get; set; } = "";
            public string TestDataParameters { get; set; } = "";
            public string EdgeResult { get; set; } = "";
            public string ChromeResult { get; set; } = "";
            public string ActualOutput { get; set; } = "";
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
            Console.WriteLine($"[AppointmentTest] {DateTime.Now:HH:mm:ss} - {message}");
        }

        /// <summary>
        /// So sánh Expected và Actual để xác định Pass/Fail
        /// Sử dụng logic: kiểm tra xem ActualOutput có chứa nội dung chính của ExpectedOutput không
        /// </summary>
        private static bool CompareExpectedActual(string expected, string actual)
        {
            if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual))
                return false;

            // Chuẩn hóa chuỗi để so sánh
            string normalizedExpected = expected.ToLower()
                .Replace("hiển thị thông báo alert:", "")
                .Replace("\"", "")
                .Trim();
            
            string normalizedActual = actual.ToLower()
                .Replace("(status:", "")
                .Replace(")", "")
                .Trim();

            // Kiểm tra nếu actual chứa các từ khóa chính từ expected
            // Trích xuất từ khóa chính (bỏ qua phần mô tả thêm)
            var expectedKeywords = normalizedExpected
                .Split(new[] { ' ', ',', '.', '!' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .Take(5)
                .ToList();

            // Đếm số từ khóa trùng khớp
            int matchCount = expectedKeywords.Count(kw => normalizedActual.Contains(kw));
            double matchRatio = expectedKeywords.Count > 0 ? (double)matchCount / expectedKeywords.Count : 0;

            // Nếu >50% từ khóa trùng -> Pass
            return matchRatio >= 0.5;
        }

        /// <summary>
        /// Thêm kết quả test vào danh sách - TỰ ĐỘNG SO SÁNH Expected vs Actual
        /// </summary>
        public static void AddTestResult(
            string id,
            string description,
            string preCondition,
            string stepsToExecute,
            string expectedOutput,
            string testDataParameters,
            string actualOutput,
            bool testPassed)
        {
            // So sánh Expected vs Actual để xác định Pass/Fail
            bool isMatch = CompareExpectedActual(expectedOutput, actualOutput);
            
            // Test phải Pass VÀ Expected phải match với Actual
            bool finalPassed = testPassed && isMatch;
            var status = finalPassed ? "Pass" : "Fail";

            // Log chi tiết ra terminal
            LogToConsole($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            LogToConsole($"📋 Test Case: {id}");
            LogToConsole($"📝 Mô tả: {description}");
            LogToConsole($"🎯 Expected: {expectedOutput}");
            LogToConsole($"📤 Actual: {actualOutput}");
            LogToConsole($"🧪 Test Assertion: {(testPassed ? "✅ PASSED" : "❌ FAILED")}");
            LogToConsole($"🔄 Expected vs Actual Match: {(isMatch ? "✅ MATCH" : "❌ NO MATCH")}");
            LogToConsole($"📊 Final Result: {(finalPassed ? "✅ PASS" : "❌ FAIL")}");
            LogToConsole($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            _results.Add(new AppointmentTestResult
            {
                Id = id,
                Items = "Đặt lịch khám",
                Description = description,
                PreCondition = preCondition,
                StepsToExecute = stepsToExecute,
                ExpectedOutput = expectedOutput,
                TestDataParameters = testDataParameters,
                EdgeResult = status,
                ChromeResult = status,
                ActualOutput = actualOutput
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
                var worksheet = workbook.Worksheets.Add("Appointment_WhiteBox_Test");

                // Định nghĩa tiêu đề cột theo format Excel
                var headers = new string[]
                {
                    "ID",
                    "Items",
                    "Description",
                    "PreCondition",
                    "Steps to Execute",
                    "Expected Output",
                    "Test Data/Parameters",
                    "Edge",
                    "Chrome",
                    "Actual Output"
                };

                // Style cho header
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
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

                    worksheet.Cell(row, 1).Value = result.Id;
                    worksheet.Cell(row, 2).Value = result.Items;
                    worksheet.Cell(row, 3).Value = result.Description;
                    worksheet.Cell(row, 4).Value = result.PreCondition;
                    worksheet.Cell(row, 5).Value = result.StepsToExecute;
                    worksheet.Cell(row, 6).Value = result.ExpectedOutput;
                    worksheet.Cell(row, 7).Value = result.TestDataParameters;
                    worksheet.Cell(row, 8).Value = result.EdgeResult;
                    worksheet.Cell(row, 9).Value = result.ChromeResult;
                    worksheet.Cell(row, 10).Value = result.ActualOutput;

                    // Style cho kết quả Pass/Fail
                    var edgeCell = worksheet.Cell(row, 8);
                    var chromeCell = worksheet.Cell(row, 9);

                    if (result.EdgeResult == "Pass")
                    {
                        edgeCell.Style.Font.FontColor = XLColor.Green;
                        edgeCell.Style.Font.Bold = true;
                    }
                    else
                    {
                        edgeCell.Style.Font.FontColor = XLColor.Red;
                        edgeCell.Style.Font.Bold = true;
                    }

                    if (result.ChromeResult == "Pass")
                    {
                        chromeCell.Style.Font.FontColor = XLColor.Green;
                        chromeCell.Style.Font.Bold = true;
                    }
                    else
                    {
                        chromeCell.Style.Font.FontColor = XLColor.Red;
                        chromeCell.Style.Font.Bold = true;
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
                worksheet.Column(1).Width = 10;   // ID
                worksheet.Column(2).Width = 15;   // Items
                worksheet.Column(3).Width = 40;   // Description
                worksheet.Column(4).Width = 40;   // PreCondition
                worksheet.Column(5).Width = 50;   // Steps to Execute
                worksheet.Column(6).Width = 40;   // Expected Output
                worksheet.Column(7).Width = 35;   // Test Data (tăng để chứa nhiều dòng)
                worksheet.Column(8).Width = 10;   // Edge
                worksheet.Column(9).Width = 10;   // Chrome
                worksheet.Column(10).Width = 40;  // Actual Output

                // Freeze header row
                worksheet.SheetView.FreezeRows(1);

                // Xử lý trường hợp file đang mở
                try
                {
                    // Xóa file cũ nếu tồn tại
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    workbook.SaveAs(filePath);
                    Console.WriteLine($"✅ Report saved at: {filePath}");
                }
                catch (IOException)
                {
                    // Nếu file đang mở, tạo file với tên mới
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
        public static int GetPassedCount() => _results.Count(r => r.EdgeResult == "Pass");

        /// <summary>
        /// Lấy số test failed
        /// </summary>
        public static int GetFailedCount() => _results.Count(r => r.EdgeResult == "Fail");
    }
}
