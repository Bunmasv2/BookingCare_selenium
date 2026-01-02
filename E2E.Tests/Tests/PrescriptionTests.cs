using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;

[TestFixture]
[Category("Prescription")]
public class PrescriptionTests : TestBase
{
    private WebDriverWait wait;
    private IJavaScriptExecutor js;
    private static int testCounter = 1;

    [SetUp]
    public void SetUp()
    {
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        js = (IJavaScriptExecutor)driver;
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void EndTest()
    {
        // Lấy tên test case
        var testName = TestContext.CurrentContext.Test.Name;

        // Lấy trạng thái (Pass/Fail)
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        string resultString = (status == NUnit.Framework.Interfaces.TestStatus.Passed) ? "pass" : "failed";

        // In ra màn hình theo định dạng yêu cầu
        Console.WriteLine($"KDT_{testCounter}_{testName} : {resultString}");

        // Tăng số thứ tự cho lần test tiếp theo
        testCounter++;
    }

    private void AddMedicine(string name, string dosage, string frequency, string duration, string usage, int expectedCount)
    {
        var medicineSearch = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("medicineSearch")));
        medicineSearch.Clear();
        medicineSearch.SendKeys(name);

        var suggestion = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath($"//div[contains(@class,'border-bottom') and contains(.,'{name}')]")
        ));
        js.ExecuteScript("arguments[0].click();", suggestion);

        driver.FindElement(By.Id("medicineDosage")).Clear();
        driver.FindElement(By.Id("medicineDosage")).SendKeys(dosage);
        driver.FindElement(By.Id("medicineFrequency")).Clear();
        driver.FindElement(By.Id("medicineFrequency")).SendKeys(frequency);
        driver.FindElement(By.Id("medicineDuration")).Clear();
        driver.FindElement(By.Id("medicineDuration")).SendKeys(duration);
        driver.FindElement(By.Id("medicineUsage")).Clear();
        driver.FindElement(By.Id("medicineUsage")).SendKeys(usage);

        var addBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(),'Thêm thuốc')]")));
        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", addBtn);
        Thread.Sleep(300);
        js.ExecuteScript("arguments[0].click();", addBtn);

        // Đợi danh sách cập nhật đủ số lượng để tránh lỗi tìm kiếm phần tử cũ
        wait.Until(d => d.FindElements(By.CssSelector(".list-group-item")).Count == expectedCount);
    }

    private void PerformLogin()
    {
        driver.Navigate().GoToUrl("http://localhost:3000/đăng%20nhập");

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='email']")));
        new LoginPage(driver).Login("r.rhm@gmail.com", "Dat@1912");

        wait.Until(d => !d.Url.ToLower().Contains("đăng"));

        driver.Navigate().GoToUrl("http://localhost:3000/thông%20tin%20cá%20nhân/#appointments");

        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("rbc-calendar")));

        // --- Logic tìm ngày khám ---
        string targetDate = "2026-01-01";
        string targetShift = "Sáng";
        string testId = $"shift-{targetDate}-{targetShift}";
        bool clicked = false;

        for (int i = 0; i < 12; i++)
        {
            var shifts = driver.FindElements(By.CssSelector($"[data-testid='{testId}']"));
            if (shifts.Count > 0)
            {
                js.ExecuteScript("arguments[0].scrollIntoView(true);", shifts[0]);
                wait.Until(ExpectedConditions.ElementToBeClickable(shifts[0])).Click();
                clicked = true;
                break;
            }
            var oldMonth = driver.FindElement(By.CssSelector(".rbc-toolbar-label")).Text;
            driver.FindElement(By.XPath("//button[normalize-space()='Next']")).Click();
            wait.Until(d => d.FindElement(By.CssSelector(".rbc-toolbar-label")).Text != oldMonth);
        }

        Assert.That(clicked, Is.True, $"Không tìm thấy ca {testId}");
        wait.Until(d => d.Url.Contains($"date={targetDate}"));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("tbody")));

        // --- Tìm bệnh nhân và bấm Kê đơn ---
        var rows = driver.FindElements(By.CssSelector("tbody tr"));
        IWebElement prescribeBtn = null;

        foreach (var row in rows)
        {
            var cells = row.FindElements(By.TagName("td"));
            if (cells.Count >= 6 && cells[1].Text.Contains("Bành Kiệt") && cells[4].Text.Contains("Đã xác nhận"))
            {
                prescribeBtn = cells[5].FindElement(By.TagName("button"));
                break;
            }
        }

        Assert.That(prescribeBtn, Is.Not.Null, "Không tìm thấy nút Kê đơn thuốc cho Bành Kiệt");
        js.ExecuteScript("arguments[0].scrollIntoView(true);", prescribeBtn);
        wait.Until(ExpectedConditions.ElementToBeClickable(prescribeBtn)).Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal.show")));
    }

    private string HandleAlert()
    {
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        var text = alert.Text;
        alert.Accept();
        return text;
    }

    //1
    [Test]
    public void Doctor_Can_Prescribe_Successfully()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Paracetamol 500mg", "1", "3", "3", "Uống sau ăn", 1);

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();
        Thread.Sleep(3000);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("thành công"));
    }

    //2 
    [Test]
    public void Cannot_Add_Medicine_When_Medicine_Is_Missing()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.SendKeys("Sâu răng nhẹ");

        driver.FindElement(By.Id("treatment"))
            .SendKeys("Vệ sinh răng miệng");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        Thread.Sleep(3000);

        Assert.That(submitBtn.Enabled, Is.False,
            "Nút Lưu đơn thuốc phải bị disable khi thiếu thuốc");
    }

    //3
    [Test]
    public void Cannot_Add_Medicine_When_Dosage_Is_Missing()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.SendKeys("Sâu răng nhẹ");

        driver.FindElement(By.Id("treatment"))
            .SendKeys("Vệ sinh răng miệng");

        var medicineSearch = driver.FindElement(By.Id("medicineSearch"));
        medicineSearch.SendKeys("Paracetamol 500mg");

        var suggestion = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//div[contains(@class,'border-bottom') and contains(.,'Para')]")
        ));
        js.ExecuteScript("arguments[0].click();", suggestion);

        driver.FindElement(By.Id("medicineUsage")).Clear();
        driver.FindElement(By.Id("medicineUsage")).SendKeys("Uống sau khi ăn");

        var addBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//button[contains(text(),'Thêm thuốc')]")
        ));

        Thread.Sleep(3000);

        Assert.That(addBtn.Enabled, Is.False,
            "Nút Thêm thuốc phải bị disable khi thiếu liều lượng");
    }

    //4
    [Test]
    public void Cannot_Prescribe_When_Medicine_Dosage_Is_Zero()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Paracetamol 500mg", "0", "3", "3", "Uống sau ăn", 1);

        // var notes = driver.FindElement(By.Id("prescriptionNotes"));
        // notes.Clear();
        // notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();
        Thread.Sleep(3000);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("bằng 0"));
    }

    //5
    [Test]
    public void Doctor_Can_Prescribe_Two_Medicines_Successfully()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Paracetamol 500mg", "1", "3", "3", "Uống sau ăn", 1);
        AddMedicine("Omeprazole 20mg", "1", "2", "7", "Uống trước khi ăn", 2);

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.CssSelector("[data-testid='btn-save']")
        ));
        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", submitBtn);
        Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", submitBtn);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("thành công"));
    }

    //6
    [Test]
    public void Cannot_Prescribe_When_Note_Is_Missing()
    {
        PerformLogin();

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Para", "1", "3", "5", "Uống sau khi ăn", 1);
        AddMedicine("Omeprazole 20mg", "1", "2", "7", "Uống trước khi ăn", 2);

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.CssSelector("[data-testid='btn-save']")
        ));
        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", submitBtn);
        Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", submitBtn);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("thành công"));
    }

    //7
    [Test]
    public void Verify_Medicine_Total_Quantity_Calculation_On_UI()
    {
        PerformLogin();

        AddMedicine("Paracetamol 500mg", "1", "3", "5", "Uống sau ăn", 1);

        var addedMedicineRow = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".list-group-item")));
        string rowText = addedMedicineRow.Text;

        Console.WriteLine($"Text trên UI: {rowText}");

        Assert.That(rowText, Does.Contain("15"),
            $"Lỗi tính toán: Mong đợi tổng số lượng là 15 (1 viên * 3 lần * 5 ngày), nhưng UI hiển thị: {rowText}");
    }

    //8
    [Test]
    public void Doctor_Can_Add_Two_Medicines_Then_Remove_First_One()
    {
        driver.Manage().Window.Maximize();
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var js = (IJavaScriptExecutor)driver;

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Para", "1", "3", "5", "Uống sau khi ăn", 1);
        AddMedicine("Omeprazole 20mg", "1", "2", "7", "Uống trước khi ăn", 2);

        var firstMedicineDeleteBtn = wait.Until(d =>
        {
            return d.FindElement(By.CssSelector(".list-group-item:first-child button.btn-danger, .list-group-item:first-child .btn-outline-danger"));
        });

        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", firstMedicineDeleteBtn);
        Thread.Sleep(300);
        js.ExecuteScript("arguments[0].click();", firstMedicineDeleteBtn);

        wait.Until(d =>
        {
            var items = d.FindElements(By.CssSelector(".list-group-item"));
            return items.Count == 1 && items[0].Text.Contains("Omeprazole 20mg");
        });

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-testid='btn-save']")));
        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", submitBtn);
        Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", submitBtn);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("thành công"));
    }

    //9
    [Test]
    public void Cannot_Prescribe_When_Not_Today()
    {
        PerformLogin();

        var diagnosis = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("diagnosis")));
        diagnosis.Clear();
        diagnosis.SendKeys("Sâu răng nhẹ");

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Paracetamol 500mg", "1", "3", "3", "Uống sau ăn", 1);

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();
        Thread.Sleep(3000);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("ngày khám"));
    }

    //10
    [Test]
    public void Cannot_Prescribe_When_Diagnosis_Is_Missing()
    {
        PerformLogin();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        var js = (IJavaScriptExecutor)driver;

        var treatment = driver.FindElement(By.Id("treatment"));
        treatment.Clear();
        treatment.SendKeys("Vệ sinh răng miệng");

        AddMedicine("Paracetamol 500mg", "1", "3", "3", "Uống sau ăn", 1);

        var notes = driver.FindElement(By.Id("prescriptionNotes"));
        notes.Clear();
        notes.SendKeys("Uống sau khi ăn 1 giờ");

        IWebElement submitBtn = wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("[data-testid='btn-save']")
        ));

        submitBtn.Click();
        Thread.Sleep(3000);

        var message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("chẩn đoán"));
    }

}
