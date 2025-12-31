using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using server.Models;
using server.Services;
using server.Controllers;
using server.DTO; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using server.Middleware;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Server.Tests
{
    [TestFixture]
    public class RegisterTests
    {
        // Các Mock Object
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<IAuth> _mockAuthService;
        private Mock<IConfiguration> _mockConfig;
        private Mock<ClinicManagementContext> _mockContext;
        private AuthController _controller;

        // Mock DbSet cho Context
        private Mock<DbSet<Patient>> _mockPatientSet;
        private Mock<DbSet<IdentityUserRole<int>>> _mockUserRoleSet;

        // Mock Data
        private const string MOCK_EMAIL = "newuser@gmail.com";
        private const string MOCK_PASS = "Dat@1912";

        // Cấu hình JSON để hiển thị tiếng Việt
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = false
        };

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Setup chung nếu cần
        }

        [SetUp]
        public void Setup()
        {
            // 1. Mock UserStore & UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            // [QUAN TRỌNG] Mock mặc định cho _userManager.Users là danh sách rỗng
            // Điều này ngăn chặn NullReferenceException khi Controller kiểm tra Phone (truy cập Users)
            // ngay cả khi đang test lỗi Email hoặc Success.
            var emptyUsers = new List<ApplicationUser>().AsQueryable();
            var mockSet = new Mock<IQueryable<ApplicationUser>>();
            mockSet.As<IAsyncEnumerable<ApplicationUser>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<ApplicationUser>(emptyUsers.GetEnumerator()));
            mockSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ApplicationUser>(emptyUsers.Provider));
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(emptyUsers.Expression);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(emptyUsers.ElementType);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(emptyUsers.GetEnumerator());
            
            _mockUserManager.Setup(x => x.Users).Returns(mockSet.Object);

            // 2. Mock SignInManager & HttpContext
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            // 3. Mock Config
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns("MotChuoiBiMatSieuDai_ItNhat32KyTu_DeTest123");

            // 4. Mock Service
            _mockAuthService = new Mock<IAuth>();

            // 5. Mock DB Context và các DbSet
            _mockPatientSet = new Mock<DbSet<Patient>>();
            _mockUserRoleSet = new Mock<DbSet<IdentityUserRole<int>>>();

            var options = new DbContextOptions<ClinicManagementContext>();
            _mockContext = new Mock<ClinicManagementContext>(options);

            _mockContext.Setup(c => c.Patients).Returns(_mockPatientSet.Object);
            _mockContext.Setup(c => c.UserRoles).Returns(_mockUserRoleSet.Object);

            // 6. Init Controller
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

        // =========================================================
        // PHẦN 1: TEST VALIDATION
        // =========================================================

        [Test]
        public void Test_Validation_EmailEmpty()
        {
            var input = new RegistryForm { 
                email = "", 
                fullname = "Test User",
                signup_password = "Password123",
                passwordConfirmed = "Password123",
                phone = "0123456789"
            };

            var expectedObj = new { ErrorMessage = "Email không được để trống!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("email"));

            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;
            TestReport.AddResult("Reg_Val_EmailEmpty", "Email: ''", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public void Test_Validation_PasswordMismatch()
        {
            var input = new RegistryForm { 
                email = "test@gmail.com", 
                fullname = "Test User",
                signup_password = "Password123",
                passwordConfirmed = "PasswordOther",
                phone = "0123456789"
            };

            var expectedObj = new { ErrorMessage = "mật khẩu xác nhận không khớp!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("passwordConfirmed"));

            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;
            TestReport.AddResult("Reg_Val_PassMismatch", "Pass != Confirm", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public void Test_Validation_PasswordTooShort()
        {
            var input = new RegistryForm { 
                email = "test@gmail.com", 
                fullname = "Test User",
                signup_password = "123",
                passwordConfirmed = "123",
                phone = "0123456789"
            };

            var expectedObj = new { ErrorMessage = "mật khẩu đăng ký tối thiểu 6 ký tự!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);

            var validationResults = ValidateModel(input);
            var error = validationResults.FirstOrDefault(x => x.MemberNames.Contains("signup_password"));

            var actualObj = new { ErrorMessage = error?.ErrorMessage ?? "No Error" };
            string actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);

            bool isPass = actualJson == expectedJson;
            TestReport.AddResult("Reg_Val_PassShort", "Pass: '123'", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        // =========================================================
        // PHẦN 2: TEST CONTROLLER LOGIC
        // =========================================================

        [Test]
        public async Task Test_Controller_EmailExists()
        {
            // Arrange
            var input = new RegistryForm { 
                email = "exist@gmail.com", 
                fullname = "User",
                signup_password = MOCK_PASS,
                passwordConfirmed = MOCK_PASS,
                phone = "0123456789"
            };

            // Mock: FindByEmailAsync trả về object (đã tồn tại)
            _mockUserManager.Setup(x => x.FindByEmailAsync(input.email))
                .ReturnsAsync(new ApplicationUser { Email = input.email });

            var expectedObj = new { StatusCode = 400, ErrorMessage = "Email đã tồn tại!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);
            string actualJson = "";
            bool isPass = false;

            // Act
            try
            {
                await _controller.Register(input);
            }
            catch (ErrorHandlingException ex)
            {
                var actualObj = new { StatusCode = ex.StatusCode, ErrorMessage = ex.ErrorMessage };
                actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);
                if (actualJson == expectedJson) isPass = true;
            }
            catch (Exception ex)
            {
                actualJson = JsonSerializer.Serialize(new { Error = "Unexpected", Detail = ex.Message }, _jsonOptions);
            }

            TestReport.AddResult("Reg_Logic_EmailExists", $"Email: {input.email}", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public async Task Test_Controller_PhoneExists()
        {
            // Arrange
            var input = new RegistryForm { 
                email = "new@gmail.com", 
                fullname = "User",
                signup_password = MOCK_PASS,
                passwordConfirmed = MOCK_PASS,
                phone = "0987654321" // Số này sẽ bị trùng
            };

            // Mock Email: Chưa tồn tại
            _mockUserManager.Setup(x => x.FindByEmailAsync(input.email)).ReturnsAsync((ApplicationUser)null);

            // [GHI ĐÈ MOCK] Tạo list user có chứa sđt trùng để test logic check Phone
            var existingUsers = new List<ApplicationUser>
            {
                new ApplicationUser { PhoneNumber = "0987654321", Email = "other@gmail.com" }
            }.AsQueryable();

            var mockSet = new Mock<IQueryable<ApplicationUser>>();
            mockSet.As<IAsyncEnumerable<ApplicationUser>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<ApplicationUser>(existingUsers.GetEnumerator()));
            mockSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ApplicationUser>(existingUsers.Provider));
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(existingUsers.Expression);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(existingUsers.ElementType);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(existingUsers.GetEnumerator());

            // Gán đè vào _userManager.Users cho test case này
            _mockUserManager.Setup(x => x.Users).Returns(mockSet.Object);

            var expectedObj = new { StatusCode = 400, ErrorMessage = "Số điện thoại đã được sử dụng!" };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);
            string actualJson = "";
            bool isPass = false;

            // Act
            try
            {
                await _controller.Register(input);
            }
            catch (ErrorHandlingException ex)
            {
                var actualObj = new { StatusCode = ex.StatusCode, ErrorMessage = ex.ErrorMessage };
                actualJson = JsonSerializer.Serialize(actualObj, _jsonOptions);
                if (actualJson == expectedJson) isPass = true;
            }
            catch (Exception ex)
            {
                actualJson = JsonSerializer.Serialize(new { Error = "Unexpected", Detail = ex.Message }, _jsonOptions);
            }

            TestReport.AddResult("Reg_Logic_PhoneExists", $"Phone: {input.phone}", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True);
        }

        [Test]
        public async Task Test_Controller_Success()
        {
            // Arrange
            var input = new RegistryForm { 
                email = MOCK_EMAIL, 
                fullname = "New User",
                signup_password = MOCK_PASS,
                passwordConfirmed = MOCK_PASS,
                phone = "0123456788"
            };

            // 1. Mock Email chưa tồn tại
            _mockUserManager.Setup(x => x.FindByEmailAsync(input.email)).ReturnsAsync((ApplicationUser)null);

            // 2. Mock Phone chưa tồn tại (List rỗng - Ghi đè lại để chắc chắn)
            var emptyUsers = new List<ApplicationUser>().AsQueryable();
            var mockSet = new Mock<IQueryable<ApplicationUser>>();
            mockSet.As<IAsyncEnumerable<ApplicationUser>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<ApplicationUser>(emptyUsers.GetEnumerator()));
            mockSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ApplicationUser>(emptyUsers.Provider));
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(emptyUsers.Expression);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(emptyUsers.ElementType);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(emptyUsers.GetEnumerator());
            _mockUserManager.Setup(x => x.Users).Returns(mockSet.Object);

            // 3. Mock CreateAsync thành công
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), input.signup_password))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((u, p) => u.Id = 123);

            // 4. Mock Add UserRoles và Patients (tránh lỗi null ref khi await AddAsync)
            _mockContext.Setup(x => x.UserRoles.AddAsync(It.IsAny<IdentityUserRole<int>>(), default))
                .ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IdentityUserRole<int>>)null);
            
            _mockContext.Setup(x => x.Patients.AddAsync(It.IsAny<Patient>(), default))
                .ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Patient>)null);

            // 5. Mock SaveChangesAsync
            _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var expectedObj = new { message = "Đăng ký thành công!", user = input.email };
            string expectedJson = JsonSerializer.Serialize(expectedObj, _jsonOptions);
            string actualJson = "";
            bool isPass = false;

            // Act
            var result = await _controller.Register(input);

            // Assert
            if (result is OkObjectResult okResult)
            {
                actualJson = JsonSerializer.Serialize(okResult.Value, _jsonOptions);
                if (actualJson == expectedJson) isPass = true;
            }
            else 
            {
                actualJson = "Result is not OkObjectResult";
            }

            TestReport.AddResult("Reg_Logic_Success", "Valid Input", expectedJson, actualJson, isPass);
            Assert.That(isPass, Is.True, $"Expected: {expectedJson}, Actual: {actualJson}");
        }
    }

    // ==================================================================================
    // HELPERS ĐỂ MOCK ASYNC QUERYABLE
    // ==================================================================================
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                                 .GetMethod(
                                     name: nameof(IQueryProvider.Execute),
                                     genericParameterCount: 1,
                                     types: new[] { typeof(Expression) })
                                 .MakeGenericMethod(expectedResultType)
                                 .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}