using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;

namespace server.Tests
{
    public class TestReportHelper
    {
        private static List<TestStepResult> _steps = new List<TestStepResult>();

        public class TestStepResult
        {
            public string TestCase { get; set; } = "";
            public string Step { get; set; } = "";
            public string Expected { get; set; } = "";
            public string Actual { get; set; } = "";
            public string Status { get; set; } = "";
        }

        public static void AddStep(string testCase, string step, string expected, string actual)
        {
            _steps.Add(new TestStepResult
            {
                TestCase = testCase,
                Step = step,
                Expected = expected,
                Actual = actual,
                Status = expected.Trim() == actual.Trim() ? "PASS" : "FAIL"
            });
        }

        public static void ExportToExcel(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("WhiteBox_Signin_Results");
                
                // Tiêu đề cột
                var headers = new string[] { "Test Case", "Step Description", "Expected Result", "Actual Result", "Status" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                for (int i = 0; i < _steps.Count; i++)
                {
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = _steps[i].TestCase;
                    worksheet.Cell(row, 2).Value = _steps[i].Step;
                    worksheet.Cell(row, 3).Value = _steps[i].Expected;
                    worksheet.Cell(row, 4).Value = _steps[i].Actual;
                    worksheet.Cell(row, 5).Value = _steps[i].Status;

                    if (_steps[i].Status == "PASS")
                        worksheet.Cell(row, 5).Style.Font.FontColor = XLColor.Green;
                    else
                        worksheet.Cell(row, 5).Style.Font.FontColor = XLColor.Red;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }
    }
}