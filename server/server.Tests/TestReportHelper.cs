using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Tests
{
    public class TestResultModel
    {
        public string TestCaseName { get; set; } = "";
        public string InputData { get; set; } = "";
        public string ExpectedResult { get; set; } = "";
        public string ActualResult { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public static class TestReport
    {
        private static List<TestResultModel> _results = new List<TestResultModel>();

        public static void AddResult(string testName, string input, string expected, string actual, bool isPass)
        {
            _results.Add(new TestResultModel
            {
                TestCaseName = testName,
                InputData = input,
                ExpectedResult = expected,
                ActualResult = actual,
                Status = isPass ? "PASS" : "FAIL"
            });
        }

        public static void ExportToExcel()
        {
            try 
            {
                var currentDir = Directory.GetCurrentDirectory();
                var projectDirInfo = Directory.GetParent(currentDir)?.Parent?.Parent;
                if (projectDirInfo == null) projectDirInfo = new DirectoryInfo(currentDir);

                var reportDir = Path.Combine(projectDirInfo.FullName, "Report");
                if (!Directory.Exists(reportDir)) Directory.CreateDirectory(reportDir);

                // Tên file theo ngày giờ
                var fileName = $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(reportDir, fileName);

                var csvContent = new StringBuilder();

                // [FIX QUAN TRỌNG] Chỉ ghi Header nếu file CHƯA tồn tại
                if (!File.Exists(filePath))
                {
                    csvContent.AppendLine("Test Case Name;Input Data;Expected Result (JSON);Actual Result (JSON);Status");
                }

                foreach (var item in _results)
                {
                    string actualRaw = item.ActualResult ?? "null";
                    string expectedRaw = item.ExpectedResult ?? "null";
                    string inputRaw = item.InputData ?? "";

                    string actualSanitized = actualRaw.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                    string expectedSanitized = expectedRaw.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                    string inputSanitized = inputRaw.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                    string testNameSanitized = item.TestCaseName.Replace(";", ",");

                    csvContent.AppendLine($"{testNameSanitized};{inputSanitized};{expectedSanitized};{actualSanitized};{item.Status}");
                }

                // [FIX QUAN TRỌNG] Dùng AppendAllText thay vì WriteAllText
                File.AppendAllText(filePath, csvContent.ToString(), new UTF8Encoding(true));
                
                Console.WriteLine($"✅ Report appended successfully to: {filePath}");
                _results.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error exporting report: {ex.Message}");
            }
        }
    }
}