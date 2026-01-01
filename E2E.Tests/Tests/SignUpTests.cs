using NUnit.Framework;
using E2E.Tests.Pages;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class SignUpTests : TestBase
{
    private SignUpPage _page;

    [SetUp]
    public new void Setup()
    {
        _page = new SignUpPage(driver);
        _page.Navigate();

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("fullName")));
    }

    [Test]
    public void DK01_SignUp_WithValidData_ShouldSuccess()
    {
        string randomEmail = $"test{DateTime.Now.Ticks}@gmail.com";
        _page.FillForm("Nguyen Van A", "0912345641", randomEmail, "User@123", "User@123");
        _page.Submit();

        Assert.That(_page.GetMessage(), Does.Contain("Đăng ký thành công!").IgnoreCase);
    }

    [Test]
    public void DK02_SignUp_WithExistingEmail_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "usertest123@gmail.com", "User@123", "User@123");
        _page.Submit();

        Assert.That(_page.GetMessage(), Does.Contain("Email đã tồn tại").IgnoreCase);
    }

    [Test]
    public void DK03_PasswordNotMatch_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "test@gmail.com", "User@123", "User@124");

        string error = _page.GetErrorMessageOfField("confirmPassword");
        Assert.That(error, Does.Contain("không khớp").IgnoreCase);
    }

    [Test]
    public void DK04_InvalidEmailFormat_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "nguyenvanb@", "User@123", "User@123");
        _page.Submit();

        string error = _page.GetErrorMessageOfField("email");
        Assert.That(error, Is.Not.Empty, "Vui lòng nhập đúng định dạng!");
    }

    [Test]
    public void DK05_EmptyEmail_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "", "User@123", "User@123");
        _page.Submit();

        string error = _page.GetErrorMessageOfField("email");
        Assert.That(error, Does.Contain("Vui lòng nhập Email").IgnoreCase);
    }

    [Test]
    public void DK06_PasswordTooShort_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "test@gmail.com", "123", "123");
        _page.Submit();

        string error = _page.GetErrorMessageOfField("password");
        Assert.That(error, Is.Not.Empty);
    }

    [Test]
    public void DK07_AllFieldsEmpty_ShouldFail()
    {
        _page.Submit();

        Assert.Multiple(() =>
        {
            Assert.That(_page.GetErrorMessageOfField("fullName"), Does.Contain("Vui lòng nhập Họ và Tên"));
            Assert.That(_page.GetErrorMessageOfField("email"), Does.Contain("Vui lòng nhập Email"));
            Assert.That(_page.GetErrorMessageOfField("phone"), Does.Contain("Vui lòng nhập Số điện thoại"));
        });
    }

    [Test]
    public void DK08_EmptyPhoneNumber_ShouldFail()
    {
        _page.FillForm("Nguyen Van B", "", "test@gmail.com", "User@123", "User@123");
        _page.Submit();

        string error = _page.GetErrorMessageOfField("phone");
        Assert.That(error, Does.Contain("Vui lòng nhập Số điện thoại").IgnoreCase);
    }

    [Test]
    public void DK09_FullNameWithSpecialChar_ShouldFail()
    {
        _page.FillForm("Nguyen Van @", "0987612345", "nguyenvanb@gmail.com", "User@123", "User@123");

        _page.Submit();

        string error = _page.GetErrorMessageOfField("fullName");

        Assert.That(error, Does.Contain("không được chứa ký tự đặc biệt").IgnoreCase, "Không hiển thị lỗi họ và tên chứa ký tự đặc biệt");
    }

    [Test]
    public void DK10_ConfirmPassword_RealtimeValidation()
    {
        _page.FillForm("Nguyen Van B", "0987612345", "test@gmail.com", "User@123", "User@124");

        Assert.That(_page.GetErrorMessageOfField("confirmPassword"), Is.Not.Empty);

        IWebElement confirmInput = driver.FindElement(By.Id("confirmPassword"));
        confirmInput.Clear();
        confirmInput.SendKeys("User@123");
        confirmInput.SendKeys(Keys.Tab);

        Assert.That(_page.GetErrorMessageOfField("confirmPassword"), Is.Empty);
    }
}