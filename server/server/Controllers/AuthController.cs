using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.DTO;
using server.Middleware;
using server.Models;
using server.Services;
using server.Util;
using servier.DTO;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ClinicManagementContext _context;
        private readonly IAuth _auth;

        public AuthController(
            ClinicManagementContext context,
            IAuth auth,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auth = auth;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new ErrorHandlingException(400, "Vui lòng nhập email!");

            // Validate email format without try-catch
            bool isValidEmail = IsValidEmail(request.Email);
            if (!isValidEmail)
                throw new ErrorHandlingException(400, "Email không hợp lệ!");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
               throw new ErrorHandlingException(400, "Email đã tồn tại trong hệ thống!");

            // Kiểm tra và rate-limit OTP
            if (!OtpUtil.CanGenerateNewOtp(request.Email))
                throw new ErrorHandlingException(400, "Vui lòng đợi 1 phút trước khi yêu cầu OTP mới!");

            // Tạo và lưu OTP mới
            var otp = OtpUtil.GenerateOtp();
            OtpUtil.StoreOtp(request.Email, otp);

            // Gửi OTP qua email
            bool emailSent = await OtpUtil.SendOtpEmail(request.Email, otp, _configuration);
            Console.WriteLine($"OTP: {otp}");
            if (!emailSent)
                throw new ErrorHandlingException(500, "Không thể gửi mã OTP. Vui lòng thử lại sau.");
            
            return Ok(new { message = "Mã OTP đã được gửi đến email của bạn!" });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] SigninForm login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email) ?? throw new ErrorHandlingException(400, "Tài khoản không tồn tại!");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!isPasswordValid)
                throw new ErrorHandlingException(400, "Sai mật khẩu!");

            var roles = await _userManager.GetRolesAsync(user);
            var token = JwtUtil.GenerateToken(user, roles, 1, _configuration);
            var refreshToken = JwtUtil.GenerateToken(user, roles, 8, _configuration);
            
            CookieUtil.SetCookie(Response, "token", token, 8);

            await _auth.SaveRefreshToken(user, refreshToken);

            return Ok(new { message = "Đăng nhập thành công!", userName = user.FullName, role = roles[0] });
        }

        private async Task<ApplicationUser?> FindUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        
        // Helper method to validate email format without try-catch
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistryForm user)
        {
            if (string.IsNullOrEmpty(user.otp))
                throw new ErrorHandlingException(400, "Vui lòng nhập mã OTP!");

            if (string.IsNullOrEmpty(user.email))
                throw new ErrorHandlingException(400, "Vui lòng nhập email!");

            // Xác thực OTP
            if (!OtpUtil.ValidateOtp(user.email, user.otp))
            {
                // Kiểm tra số lần thử và xử lý
                if (OtpUtil.MaxAttemptsReached(user.email))
                {
                    OtpUtil.RemoveOtp(user.email);
                    throw new ErrorHandlingException(400, "Bạn đã nhập sai OTP quá nhiều lần. Vui lòng yêu cầu mã mới!");
                }
                throw new ErrorHandlingException(400, "Mã OTP không hợp lệ hoặc đã hết hạn!");
            }

            var existUser = await _userManager.FindByEmailAsync(user.email);
            var existPhone = await FindUserByPhoneNumberAsync(user.phone);

            if (existUser != null)
               throw new ErrorHandlingException(400, "Email đã tồn tại!");

            if (existPhone != null)
               throw new ErrorHandlingException(400, "Số điện thoại đã được sử dụng!");

            var newUser = new ApplicationUser
            {
                UserName = user.email,
                Email = user.email,
                PhoneNumber = user.phone,
                FullName = user.fullname
            };

            var result = await _userManager.CreateAsync(newUser, user.signup_password);
            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                throw new ErrorHandlingException(400, firstError?.Description ?? "Đăng ký thất bại");
            }

            Console.WriteLine("User created successfully!:"+ newUser.Id);

            await _context.UserRoles.AddAsync(new IdentityUserRole<int>
            {
                UserId = newUser.Id,
                RoleId = 3 // role bệnh nhân
            });

            await _context.Patients.AddAsync(new Patient { UserId = newUser.Id });

            await _context.SaveChangesAsync();

            // Xóa OTP sau khi đã sử dụng thành công
            OtpUtil.RemoveOtp(user.email);

            return Ok(new { message = "Đăng ký thành công!", user = newUser.Email });
        }

        [Authorize(Roles = "doctor")]
        [HttpPost("auth_user")]
        public IActionResult AuthUser([FromBody] LoginForm user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                throw new ErrorHandlingException(400, "Vui lòng nhập đủ thông tin!");

            return Ok(new { Token = "HttpContext", message = "Xác thực thành công", user });
        }

        // Gửi OTP để reset mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new ErrorHandlingException(400, "Vui lòng nhập email!");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new ErrorHandlingException(400, "Email không tồn tại trong hệ thống!");

            // rate‐limit: chặn yêu cầu mới trong 1 phút
            if (!OtpUtil.CanGenerateNewOtp(request.Email))
                throw new ErrorHandlingException(400, "Vui lòng chờ ít nhất 1 phút trước khi yêu cầu mã mới!");

            // Tạo & lưu OTP
            var otp = OtpUtil.GenerateOtp();
            OtpUtil.StoreOtp(request.Email, otp);

            // Gửi email OTP với nội dung "reset password"
            bool emailSent = await OtpUtil.SendResetPasswordEmail(request.Email, otp, _configuration);
            if (!emailSent)
                throw new ErrorHandlingException(500, "Không thể gửi mã OTP. Vui lòng thử lại sau.");

            return Ok(new { message = "Mã OTP đã được gửi đến email của bạn!" });
        }

        // Xác thực OTP và đổi mật khẩu
        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] ResetPasswordForm request)
        {
            if (string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.Code)
                || string.IsNullOrEmpty(request.NewPassword))
            {
                throw new ErrorHandlingException(400, "Thiếu thông tin!");
            }

            var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new ErrorHandlingException(400, "Email không tồn tại trong hệ thống!");

            // Kiểm tra OTP
            if (!OtpUtil.ValidateOtp(request.Email, request.Code))
            {
                if (OtpUtil.MaxAttemptsReached(request.Email))
                {
                    OtpUtil.RemoveOtp(request.Email);
                    throw new ErrorHandlingException(400, "Bạn đã nhập sai OTP quá nhiều lần. Vui lòng yêu cầu mã mới!");
                }
                throw new ErrorHandlingException(400, "Mã OTP không hợp lệ hoặc đã hết hạn!");
            }

            // Dùng Identity token để reset password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
            if (!result.Succeeded)
                throw new ErrorHandlingException(500, "Cập nhật mật khẩu thất bại!");

            // Xóa OTP sau khi thành công
            OtpUtil.RemoveOtp(request.Email);

            return Ok(new { message = "Đặt lại mật khẩu thành công!" });
        }
    }
}