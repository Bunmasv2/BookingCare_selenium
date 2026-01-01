using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E2E.Tests.Pages
{
    public class SignInPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public SignInPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement EmailInput => _driver.FindElement(By.Id("emailLogin"));
        private IWebElement PasswordInput => _driver.FindElement(By.Id("passwordLogin"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login-button"));

        public void Navigate()
        {
            _driver.Navigate().GoToUrl("http://localhost:3000/đăng%20nhập");
        }

        public void FillForm(string email, string password)
        {
            EmailInput.Clear();
            if (email != null) EmailInput.SendKeys(email);

            PasswordInput.Clear();
            if (password != null) PasswordInput.SendKeys(password);
        }

        public void Submit()
        {
            LoginButton.Click();
        }

        public string GetAlertMessage()
        {
            try
            {
                IAlert alert = _wait.Until(ExpectedConditions.AlertIsPresent());
                string text = alert.Text;
                alert.Accept();
                return text;
            }
            catch (WebDriverTimeoutException) { return ""; }
        }

        public string GetFieldErrorMessage(string fieldId)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                By errorLocator = By.XPath($"//input[@id='{fieldId}']/following-sibling::div[contains(@class, 'invalid-feedback')]");

                IWebElement errorElement = wait.Until(d =>
                {
                    var el = d.FindElement(errorLocator);
                    return (el.Displayed && !string.IsNullOrEmpty(el.Text.Trim())) ? el : null;
                });

                return errorElement.Text.Trim();
            }
            catch (WebDriverTimeoutException)
            {
                return "";
            }
        }
    }
}