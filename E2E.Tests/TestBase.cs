using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class TestBase
{
    protected IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();

        options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
        options.AddUserProfilePreference("credentials_enable_service", false);
        options.AddUserProfilePreference("profile.password_manager_enabled", false);
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-popup-blocking");

        driver = new ChromeDriver(options);
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Quit();
        driver?.Dispose();
    }
}
