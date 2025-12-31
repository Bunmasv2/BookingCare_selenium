using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;

[TestFixture]
[Category("Prescription")]
public class PrescriptionTests : TestBase
{
    private void PerformLogin()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        driver.Navigate().GoToUrl("http://localhost:3000/đăng%20nhập");

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("input[type='email']")
        ));

        new LoginPage(driver).Login("r.rhm@gmail.com", "Dat@1912");

        wait.Until(d => !d.Url.ToLower().Contains("đăng"));

        driver.Navigate().GoToUrl(
            "http://localhost:3000/thông%20tin%20cá%20nhân/#appointments"
        );

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.ClassName("rbc-calendar")
        ));

        string targetDate = "2026-01-01";
        string targetShift = "Sáng";
        string testId = $"shift-{targetDate}-{targetShift}";

        var nextBtn = By.XPath("//button[normalize-space()='Next']");
        var monthLabel = By.CssSelector(".rbc-toolbar-label");

        bool clicked = false;

        for (int i = 0; i < 12; i++)
        {
            var shifts = driver.FindElements(
                By.CssSelector($"[data-testid='{testId}']")
            );

            if (shifts.Count > 0)
            {
                ((IJavaScriptExecutor)driver)
                    .ExecuteScript("arguments[0].scrollIntoView(true);", shifts[0]);

                wait.Until(ExpectedConditions.ElementToBeClickable(shifts[0])).Click();
                clicked = true;
                break;
            }

            string oldMonth = driver.FindElement(monthLabel).Text;
            driver.FindElement(nextBtn).Click();

            wait.Until(d => d.FindElement(monthLabel).Text != oldMonth);
        }

        Assert.That(clicked, Is.True, $"Không tìm thấy ca {testId}");

        wait.Until(d => d.Url.Contains($"date={targetDate}"));

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("tbody")
        ));

        var rows = driver.FindElements(By.CssSelector("tbody tr"));

        IWebElement targetRow = null;
        IWebElement prescribeBtn = null;

        foreach (var row in rows)
        {
            try
            {
                var cells = row.FindElements(By.TagName("td"));
                if (cells.Count < 6) continue;

                string patientName = cells[1].Text.Trim();
                string status = cells[4].Text.Trim();

                if (patientName.Contains("Bành Kiệt") && status.Contains("Đã xác nhận"))
                {
                    targetRow = row;
                    prescribeBtn = cells[5].FindElement(By.TagName("button"));
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi quét hàng: " + ex.Message);
            }
        }

        // Assert.That(targetRow, Is.Not.Null, "Không tìm thấy lịch hẹn của Bành Kiệt với trạng thái Đã xác nhận");
        // Assert.That(prescribeBtn, Is.Not.Null, "Không tìm thấy nút Kê đơn thuốc trong hàng của Bành Kiệt");

        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", prescribeBtn);
        wait.Until(ExpectedConditions.ElementToBeClickable(prescribeBtn)).Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal.show")));
    }

    private string HandleAlert()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        var text = alert.Text;
        alert.Accept();
        return text;
    }

    [Test]
    public void Cannot_Prescribe_When_Not_Today()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Cảm cúm thông thường");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Nghỉ ngơi, uống đủ nước, dùng thuốc theo chỉ định");

        var medicineSearch = driver.FindElement(By.Id("medicineSearch"));
        medicineSearch.Clear();
        medicineSearch.SendKeys("Para");

        var firstSuggestion = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//div[contains(@class,'border-bottom') and contains(.,'Para')]")
        ));

        js.ExecuteScript("arguments[0].click();", firstSuggestion);

        var dosage = driver.FindElement(By.Id("medicineDosage"));
        dosage.Clear();
        dosage.SendKeys("1");

        var frequency = driver.FindElement(By.Id("medicineFrequency"));
        frequency.Clear();
        frequency.SendKeys("3");

        var duration = driver.FindElement(By.Id("medicineDuration"));
        duration.Clear();
        duration.SendKeys("5");

        var usage = driver.FindElement(By.Id("medicineUsage"));
        usage.Clear();
        usage.SendKeys("Uống sau khi ăn");

        var addMedicineBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(text(),'Thêm thuốc')]")
        ));

        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", addMedicineBtn);
        Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", addMedicineBtn);

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector(".list-group-item")
        ));

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Theo dõi triệu chứng, tái khám nếu không đỡ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("ngày khám"));
    }

    [Test]
    public void Cannot_Prescribe_When_Medicine_Dosage_Is_Zero()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Cảm cúm thông thường");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Nghỉ ngơi, uống đủ nước, dùng thuốc theo chỉ định");

        var medicineSearch = driver.FindElement(By.Id("medicineSearch"));
        medicineSearch.Clear();
        medicineSearch.SendKeys("Para");

        var firstSuggestion = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//div[contains(@class,'border-bottom') and contains(.,'Para')]")
        ));

        js.ExecuteScript("arguments[0].click();", firstSuggestion);

        var dosage = driver.FindElement(By.Id("medicineDosage"));
        dosage.Clear();
        dosage.SendKeys("0");

        var frequency = driver.FindElement(By.Id("medicineFrequency"));
        frequency.Clear();
        frequency.SendKeys("3");

        var duration = driver.FindElement(By.Id("medicineDuration"));
        duration.Clear();
        duration.SendKeys("5");

        var usage = driver.FindElement(By.Id("medicineUsage"));
        usage.Clear();
        usage.SendKeys("Uống sau khi ăn");

        var addMedicineBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(text(),'Thêm thuốc')]")
        ));

        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", addMedicineBtn);
        Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", addMedicineBtn);

        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector(".list-group-item")
        ));

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Theo dõi triệu chứng, tái khám nếu không đỡ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("bằng 0"));
    }

    // [Test]
    // public void Doctor_Can_Prescribe_Successfully()
    // {
    //     LoginAsDoctor();

    //     FillValidPrescriptionForm();
    //     driver.FindElement(By.CssSelector("button[type='submit']")).Click();

    //     var message = HandleAlert();
    //     Assert.That(message.ToLower(), Does.Contain("thành công"));
    // }
}
