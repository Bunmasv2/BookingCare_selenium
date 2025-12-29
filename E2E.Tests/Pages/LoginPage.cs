using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class LoginPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    private IWebElement EmailInput =>
        wait.Until(d =>
        {
            var el = d.FindElement(By.Name("email"));
            return el.Displayed ? el : null;
        });

    private IWebElement PasswordInput =>
        wait.Until(d =>
        {
            var el = d.FindElement(By.CssSelector("input[type='password']"));
            return el.Displayed ? el : null;
        });

    private IWebElement LoginButton =>
        wait.Until(d =>
        {
            var el = d.FindElement(By.CssSelector("button[type='submit']"));
            return el.Enabled ? el : null;
        });

    public void Login(string email, string password)
    {
        EmailInput.Clear();
        EmailInput.SendKeys(email);
        PasswordInput.Clear();
        PasswordInput.SendKeys(password);

        LoginButton.Click();

        // Trong code React của bạn: navigate("/") sau khi thành công
        // Đợi cho đến khi URL trở về trang chủ hoặc trang Appointment
        wait.Until(d => d.Url == "http://localhost:3000/" || d.Url.Contains("/đặt%20lịch%20khám"));
    }
}