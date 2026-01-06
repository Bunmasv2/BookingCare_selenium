using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using server.Models;
using server.Services;
using server.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using server.Middleware;
using System.Text.Json;
using System.Text.Encodings.Web; // [MỚI] Cần thư viện này để hiển thị tiếng Việt
using System.Text.Unicode;     // [MỚI]

namespace Server.Tests
{
    [TestFixture]
    public class SigninTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<IAuth> _mockAuthService;
        private Mock<IConfiguration> _mockConfig;
        private Mock<ClinicManagementContext> _mockContext;
        private AuthController _controller;

        private ApplicationUser _mockUser;
        private const string MOCK_EMAIL = "r.rhm@gmail.com";
        private const string MOCK_PASS_RAW = "Dat@1912";
        private const string MOCK_PASS_HASH = "AQAAAAIAAYagAAAAEG0luV9tYiu6rB/3VTb8iqRmV2Ee+zgwgvmc9AsMZWVPEXiU7tvODG2UE4c8dwDykA==";
        private const string MOCK_ROLE = "Doctor";

        // [FIX QUAN TRỌNG] Cấu hình để JSON không bị lỗi font tiếng Việt
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // Hiển thị mọi ký tự Unicode (Tiếng Việt)
            WriteIndented = false // Không xuống dòng để tiết kiệm diện tích trong Excel
        };

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            _mockUser = new ApplicationUser
            {
                Id = 1,
                FullName = "abc",
                UserName = "abc",
                Email = MOCK_EMAIL,
                PasswordHash = MOCK_PASS_HASH
            };
        }

        [SetUp]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns("MotChuoiBiMatSieuDai_ItNhat32KyTu_DeTest123");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("http://localhost:5000");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("http://localhost:3000");

            _mockAuthService = new Mock<IAuth>();
            var options = new DbContextOptions<ClinicManagementContext>();
            _mockContext = new Mock<ClinicManagementContext>(options);

            _controller = new AuthController(
                _mockContext.Object,
                _mockAuthService.Object,
                _mockConfig.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object
            );

            _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TestReport.ExportToExcel();
        }

        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        // --- CÁC TEST CASE ---

        [Test]
        public void Test_Validation_EmailEmpty()
        {
            var input = new SigninForm { Email = "", Password = "123" };
            
            var expectedObj = new { ErrorMessage = "Email Không được bỏ trống!" };
            // [FIX] Truyền _jsonOptions vào đây
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("Email"));
            
            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            // [FIX] Truyền _jsonOptions vào đây
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;

            TestReport.AddResult("Validate Empty Email", "Email: ''", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True, $"JSON Mismatch! Expected: {expectedJson}, Actual: {actualJson}");
        }

        [Test]
        public void Test_Validation_EmailInvalidFormat()
        {
            var input = new SigninForm { Email = "invalid-email", Password = "123" };

            var expectedObj = new { ErrorMessage = "Vui lòng nhập đúng định dạng!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("Email"));

            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;

            TestReport.AddResult("Validate Invalid Email", "Email: 'invalid-email'", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public void Test_Validation_PasswordEmpty()
        {
            var input = new SigninForm { Email = "test@gmail.com", Password = "" };

            var expectedObj = new { ErrorMessage = "Password Is Required!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("Password"));

            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;

            TestReport.AddResult("Validate Empty Password", "Pass: ''", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public async Task Test_Controller_UserNotFound()
        {
            var input = new SigninForm { Email = "notfound@gmail.com", Password = "123" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(input.Email)).ReturnsAsync((ApplicationUser)null);

            var expectedObj = new { StatusCode = 400, ErrorMessage = "Tài khoản không tồn tại!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            string actualJson = "";
            bool isPass = false;

            try
            {
                await _controller.Signin(input);
            }
            catch (ErrorHandlingException ex) 
            {
                var actualObj = new { StatusCode = ex.StatusCode, ErrorMessage = ex.ErrorMessage };
                actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

                if (actualJson == expectedJson) isPass = true;
            }
            catch (Exception ex)
            {
                actualJson = JsonSerializer.Serialize(new { Error = "Unexpected Exception", Detail = ex.Message }, _jsonOptions);
            }

            TestReport.AddResult("Controller User Not Found", $"Email: {input.Email}", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True, $"JSON Mismatch! Expected: {expectedJson}, Actual: {actualJson}");
        }

        [Test]
        public async Task Test_Controller_WrongPassword()
        {
            var input = new SigninForm { Email = MOCK_EMAIL, Password = "WrongPassword" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(input.Email)).ReturnsAsync(_mockUser);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(_mockUser, input.Password)).ReturnsAsync(false);

            var expectedObj = new { StatusCode = 400, ErrorMessage = "Sai mật khẩu!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            string actualJson = "";
            bool isPass = false;

            try
            {
                await _controller.Signin(input);
            }
            catch (ErrorHandlingException ex)
            {
                var actualObj = new { StatusCode = ex.StatusCode, ErrorMessage = ex.ErrorMessage };
                actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

                if (actualJson == expectedJson) isPass = true;
            }

            TestReport.AddResult("Controller Wrong Password", $"Pass: {input.Password}", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public async Task Test_Controller_Success()
        {
            var input = new SigninForm { Email = MOCK_EMAIL, Password = MOCK_PASS_RAW };

            _mockUserManager.Setup(x => x.FindByEmailAsync(input.Email)).ReturnsAsync(_mockUser);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(_mockUser, input.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(_mockUser)).ReturnsAsync(new List<string> { MOCK_ROLE });
            _mockAuthService.Setup(x => x.SaveRefreshToken(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var expectedObj = new { message = "Đăng nhập thành công!", userName = "abc", role = "Doctor" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            string actualJson = "";
            bool isPass = false;

            var result = await _controller.Signin(input);

            if (result is OkObjectResult okResult)
            {
                actualJson = JsonSerializer.Serialize(okResult.Value, _jsonOptions);
                if (actualJson == expectedJson) isPass = true;
            }
            else 
            {
                actualJson = "Result is not OkObjectResult";
            }

            TestReport.AddResult("Controller Login Success", "Valid User", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True, $"JSON Mismatch! Expected: {expectedJson}, Actual: {actualJson}");
        }
    }
}