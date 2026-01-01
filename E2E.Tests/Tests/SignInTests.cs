using NUnit.Framework;
using E2E.Tests.Pages;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.CodeAnalysis;

[TestFixture]
public class SignInTests : TestBase
{
    private SignInPage _page;

    [SetUp]
    public new void Setup()
    {
        _page = new SignInPage(driver);
        _page.Navigate();
    }

    [Test]
    public void DN01_Login_Success()
    {
        _page.FillForm("Admin@gmail.com", "Admin2025@");
        _page.Submit();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        wait.Until(d => d.Url == "http://localhost:3000/");
        Assert.That(driver.Url, Is.EqualTo("http://localhost:3000/"));
    }

    [Test]
    public void DN02_EmptyEmail_ShouldFail()
    {
        _page.FillForm("", "Admin2025@");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("emailLogin"), Does.Contain("Vui lòng nhập Email").IgnoreCase);
    }

    [Test]
    public void DN03_EmptyPassword_ShouldFail()
    {
        _page.FillForm("Admin@gmail.com", "");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("passwordLogin"), Does.Contain("Vui lòng nhập Mật khẩu ").IgnoreCase);
    }

    [Test]
    public void DN04_EmailNotExist_ShouldFail()
    {
        _page.FillForm("random123@gmail.com", "Random@123");
        _page.Submit();
        Assert.That(_page.GetAlertMessage(), Does.Contain("Tài khoản không tồn tại").IgnoreCase);
    }

    [Test]
    public void DN05_WrongPassword_ShouldFail()
    {
        _page.FillForm("Admin@gmail.com", "Admin2000@");
        _page.Submit();
        Assert.That(_page.GetAlertMessage(), Does.Contain("Sai mật khẩu").IgnoreCase);
    }

    [Test]
    public void DN06_EmptyBothFields_ShouldFail()
    {
        _page.FillForm("", "");
        _page.Submit();
        Assert.Multiple(() =>
        {
            Assert.That(_page.GetFieldErrorMessage("emailLogin"), Does.Contain("Vui lòng nhập Email").IgnoreCase);
            Assert.That(_page.GetFieldErrorMessage("passwordLogin"), Does.Contain("Vui lòng nhập Mật khẩu đăng nhập").IgnoreCase);
        });
    }

    [Test]
    public void DN07_InvalidPasswordFormat_ShouldFail()
    {
        _page.FillForm("admin@gmail.com", "admin123");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("passwordLogin"), Does.Contain("ký tự đặc biệt").Or.Contain("chữ hoa"));
    }

    [Test]
    public void DN08_InvalidEmailFormat_ShouldFail()
    {
        _page.FillForm("admin@gmail", "Admin2025@");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("emailLogin"), Does.Contain("Email không đúng định dạng").IgnoreCase);
    }

    [Test]
    public void DN09_PasswordTooShort_ShouldFail()
    {
        _page.FillForm("admin@gmail.com", "Ad@12");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("passwordLogin"), Does.Contain("ít nhất 6 ký tự").IgnoreCase);
    }

    [Test]
    public void DN10_EmailWithSpaces_ShouldFail()
    {
        _page.FillForm("admin @gmail.com", "Admin2025@");
        _page.Submit();
        Assert.That(_page.GetFieldErrorMessage("emailLogin"), Does.Contain("Phần đứng sau '@' không được chứa khoảng trắng!").IgnoreCase);
    }
}