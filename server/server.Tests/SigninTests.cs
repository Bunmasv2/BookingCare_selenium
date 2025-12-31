using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using server.Controllers;
using server.Middleware;
using server.Models;
using server.Services;

namespace server.Tests
{
    [TestFixture]
    public class SigninTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfig;
        private Mock<IAuth> _mockAuth;
        private AuthController _controller;
        private string _reportPath;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Xác định thư mục Report nằm cùng cấp với file SigninTests.cs
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string reportFolder = Path.Combine(projectDirectory, "Report");

            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder);
            }

            _reportPath = Path.Combine(reportFolder, "WhiteBox_Signin_Report.xlsx");
        }

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns("MộtPassphraseDàiÍtNhất32KýTự1234567890");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("http://localhost:5140");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("http://localhost:3000");

            _mockAuth = new Mock<IAuth>();

            // Controller nhận Mock objects
            _controller = new AuthController(null!, _mockAuth.Object, _mockConfig.Object, null!, _mockUserManager.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Test]
        public async Task Signin_WhiteBox_CompleteTest()
        {
            // --- CASE 1: PHỦ NHÁNH TÀI KHOẢN KHÔNG TỒN TẠI ---
            string tc1 = "TC01: Account Not Found";
            var login1 = new SigninForm { Email = "notfound@gmail.com", Password = "123" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("notfound@gmail.com")).ReturnsAsync((ApplicationUser)null!);

            TestReportHelper.AddStep(tc1, "Kiểm tra sự tồn tại của Email", "Throw ErrorHandlingException", "Exception Thrown");
            var ex1 = Assert.ThrowsAsync<ErrorHandlingException>(() => _controller.Signin(login1));
            TestReportHelper.AddStep(tc1, "So khớp thông báo lỗi", "Tài khoản không tồn tại!", ex1!.ErrorMessage);

            // --- CASE 2: PHỦ NHÁNH SAI MẬT KHẨU ---
            string tc2 = "TC02: Invalid Password";
            var login2 = new SigninForm { Email = "user@test.com", Password = "wrong_password" };
            var user2 = new ApplicationUser { Email = "user@test.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("user@test.com")).ReturnsAsync(user2);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user2, "wrong_password")).ReturnsAsync(false);

            TestReportHelper.AddStep(tc2, "Kiểm tra mật khẩu", "Throw ErrorHandlingException", "Exception Thrown");
            var ex2 = Assert.ThrowsAsync<ErrorHandlingException>(() => _controller.Signin(login2));
            TestReportHelper.AddStep(tc2, "So khớp thông báo lỗi", "Sai mật khẩu!", ex2!.ErrorMessage);

            // --- CASE 3: PHỦ ĐƯỜNG THÀNH CÔNG (TOKEN & COOKIE) ---
            string tc3 = "TC03: Signin Success";
            var login3 = new SigninForm { Email = "valid@test.com", Password = "Password123" };
            var user3 = new ApplicationUser { Id = 1, FullName = "Nguyễn Văn A", Email = "valid@test.com" };
            var roles3 = new List<string> { "doctor" };

            _mockUserManager.Setup(x => x.FindByEmailAsync("valid@test.com")).ReturnsAsync(user3);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user3, "Password123")).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user3)).ReturnsAsync(roles3);

            var result = await _controller.Signin(login3);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var responseData = (result as OkObjectResult)!.Value;
            var userName = responseData?.GetType().GetProperty("userName")?.GetValue(responseData, null)?.ToString();

            TestReportHelper.AddStep(tc3, "Kiểm tra dữ liệu UserName", "Nguyễn Văn A", userName!);

            string cookie = _controller.Response.Headers["Set-Cookie"].ToString();
            TestReportHelper.AddStep(tc3, "Kiểm tra thiết lập Cookie", "True", cookie.Contains("token=").ToString());

            _mockAuth.Verify(a => a.SaveRefreshToken(user3, It.IsAny<string>()), Times.Once);
            TestReportHelper.AddStep(tc3, "Xác minh lưu RefreshToken", "Called Once", "Called Once");

            // --- XUẤT FILE ---
            TestReportHelper.ExportToExcel(_reportPath);
            TestContext.Out.WriteLine($"Success! Report saved at: {_reportPath}");
        }
    }
}