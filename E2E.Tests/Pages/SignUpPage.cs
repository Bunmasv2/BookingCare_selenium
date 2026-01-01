using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace E2E.Tests.Pages
{
    public class SignUpPage
    {
        private readonly IWebDriver _driver;
        private IReadOnlyCollection<IWebElement> ErrorMessages => _driver.FindElements(By.CssSelector(".invalid-feedback"));

        public SignUpPage(IWebDriver driver)
        {
            _driver = driver;
        }

        private IWebElement FullName => _driver.FindElement(By.Id("fullName"));
        private IWebElement Phone => _driver.FindElement(By.Id("phone"));
        private IWebElement Email => _driver.FindElement(By.Id("email"));
        private IWebElement Password => _driver.FindElement(By.Id("password"));
        private IWebElement ConfirmPassword => _driver.FindElement(By.Id("confirmPassword"));
        private IWebElement SignUpButton => _driver.FindElement(By.Id("btnSignUp"));

        private IWebElement AlertMessage =>
            _driver.FindElement(By.CssSelector(".alert"));

        public void Navigate()
        {
            _driver.Navigate().GoToUrl("http://localhost:3000/đăng%20nhập");
            var btnSwitch = _driver.FindElement(By.ClassName("toggle-btn"));
            btnSwitch.Click();
        }

        public string GetErrorMessageOfField(string fieldId)
        {
            try
            {
                var xpath = $"//input[@id='{fieldId}']/following-sibling::div[contains(@class, 'invalid-feedback')]";
                return _driver.FindElement(By.XPath(xpath)).Text;
            }
            catch (NoSuchElementException)
            {
                return "";
            }
        }

        public void FillForm(string fullName, string phone, string email, string password, string confirmPassword)
        {
            if (fullName != null) FullName.SendKeys(fullName);
            if (phone != null) Phone.SendKeys(phone);
            if (email != null) Email.SendKeys(email);
            if (password != null) Password.SendKeys(password);
            if (confirmPassword != null) ConfirmPassword.SendKeys(confirmPassword);
        }

        public void Submit()
        {
            SignUpButton.Click();
        }
        public string GetMessage()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                string alertText = alert.Text;

                alert.Accept();
                return alertText;
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    var htmlAlert = _driver.FindElement(By.CssSelector(".alert"));
                    return htmlAlert.Text;
                }
                catch (NoSuchElementException)
                {
                    return "Không tìm thấy thông báo nào";
                }
            }
        }
    }
}
