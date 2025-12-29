using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;

public class AppointmentTests : TestBase
{
    private void PerformLogin()
    {
        driver.Navigate().GoToUrl("http://localhost:3000/đăng%20nhập");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        wait.Until(ExpectedConditions.ElementIsVisible(
            By.CssSelector("input[type='email']")
        ));

        var loginPage = new LoginPage(driver);
        loginPage.Login("banhtuankiet2908@gmail.com", "Tuan2908Kiet@");

        wait.Until(d =>
            !string.IsNullOrEmpty(d.Url) &&
            !d.Url.ToLower().Contains("đăng")
        );

        driver.Navigate().GoToUrl("http://localhost:3000/đặt%20lịch%20khám");

        wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("form")));
    }
    private string HandleAlert()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        var alert = wait.Until(ExpectedConditions.AlertIsPresent());
        string text = alert.Text;
        alert.Accept();
        return text;
    }

    private void FillValidBaseForm(AppointmentPage page)
    {
        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.SelectDepartment("Khoa Nội tổng quát");

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#doctor option")).Count > 1);

        page.SelectDoctor("TRẦN HỮU LỢI");

        page.SelectService("Khám tổng quát");
        page.SelectTime("Sáng");
    }

    [Test]
    public void Cannot_Book_Appointment_Without_Login()
    {
        driver.Navigate().GoToUrl("http://localhost:3000/đặt%20lịch%20khám");
        var page = new AppointmentPage(driver);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.Submit();

        string alertText = HandleAlert();
        Assert.That(alertText.ToLower(), Does.Contain("đăng"));

        var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
        wait.Until(d =>
        {
            try
            {
                string currentUrl = System.Uri.UnescapeDataString(d.Url).ToLower();
                return currentUrl.Contains("đăng nhập");
            }
            catch
            {
                return false;
            }
        });

        Assert.That(System.Uri.UnescapeDataString(driver.Url).ToLower(), Does.Contain("đăng nhập"));
    }

    [Test]
    public void Cannot_Book_When_Department_Is_Missing()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        // page.SelectDoctor("TRẦN HỮU LỢI");
        // page.SelectService("Khám tổng quát");
        // page.SelectTime("Sáng");
        // page.SelectDate("2025-12-30");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("khoa"));
    }

    [Test]
    public void Cannot_Book_When_Doctor_Is_Missing()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.SelectDepartment("Khoa Nội tổng quát");

        page.SelectService("Khám tổng quát");
        page.SelectTime("Sáng");
        page.SelectDate("2025-12-30");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("bác sĩ"));
    }

    [Test]
    public void Cannot_Book_When_Service_Is_Missing()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.SelectDepartment("Khoa Nội tổng quát");

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#doctor option")).Count > 1);

        page.SelectDoctor("TRẦN HỮU LỢI");

        page.SelectTime("Sáng");
        page.SelectDate("2025-12-30");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("dịch vụ"));
    }

    [Test]
    public void Cannot_Book_When_Date_Is_Missing()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        FillValidBaseForm(page);

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("ngày"));
    }

    [Test]
    public void Cannot_Book_When_Time_Is_Missing()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.SelectDepartment("Khoa Nội tổng quát");

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#doctor option")).Count > 1);

        page.SelectDoctor("TRẦN HỮU LỢI");

        page.SelectService("Khám tổng quát");
        page.SelectDate("2025-12-30");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("buổi"));
    }

    public void Cannot_Book_When_Symptoms_Too_Long()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#department option")).Count > 1);

        page.SelectDepartment("Khoa Nội tổng quát");

        new WebDriverWait(driver, TimeSpan.FromSeconds(8))
            .Until(d => d.FindElements(By.CssSelector("#doctor option")).Count > 1);

        page.SelectDoctor("TRẦN HỮU LỢI");

        page.SelectService("Khám tổng quát");
        page.SelectDate("2025-12-30");
        page.SelectSymptoms("Gần đây tôi thường xuyên cảm thấy mệt mỏi, uể oải dù không làm việc quá sức. Giấc ngủ không ổn định, khó ngủ hoặc ngủ không sâu, khiến tinh thần thiếu tỉnh táo vào ban ngày. Tôi đôi lúc bị đau đầu nhẹ, choáng váng khi đứng lên đột ngột và khả năng tập trung giảm. Ngoài ra, tôi nhận thấy ăn uống kém ngon miệng, thỉnh thoảng đầy bụng, khó tiêu. Cơ thể có dấu hiệu suy giảm sức đề kháng, dễ cảm cúm hơn trước. Do công việc và sinh hoạt có nhiều áp lực kéo dài, tôi lo ngại sức khỏe tổng thể có thể đang gặp vấn đề tiềm ẩn. Vì vậy, tôi mong muốn được khám sức khỏe tổng quát để kiểm tra toàn diện, phát hiện sớm các nguy cơ và có hướng điều chỉnh phù hợp.");
        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("dài"));
    }

    [Test]
    public void Cannot_Book_In_The_Past()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        FillValidBaseForm(page);
        page.SelectDate("2020-01-01");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message, Does.Contain("tối thiểu trước 1 ngày"));
    }

    [Test]
    public void Patient_Can_Book_Appointment_Successfully()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        FillValidBaseForm(page);
        page.SelectDate("2025-12-30");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message.ToLower(), Does.Contain("thành công"));
    }

    [Test]
    public void Cannot_Book_More_Than_15_Days()
    {
        PerformLogin();
        var page = new AppointmentPage(driver);

        FillValidBaseForm(page);
        page.SelectDate("2026-02-02");

        page.Submit();

        string message = HandleAlert();
        Assert.That(message, Does.Contain("15 ngày"));
    }
}
