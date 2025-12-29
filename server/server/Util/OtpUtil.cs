using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace server.Util
{
    public class OtpUtil
    {
        private static readonly ConcurrentDictionary<string, OtpInfo> _otpDictionary = new();

        public class OtpInfo
        {
            public string Code { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime ExpiryTime { get; set; }
            public int AttemptCount { get; set; }
        }

        public static string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] data = new byte[4];
            rng.GetBytes(data);
            int value = BitConverter.ToInt32(data, 0) & 0x7FFFFFFF;
            return (value % 1000000).ToString("D6");
        }

        public static void StoreOtp(string email, string otp, int expiryMinutes = 5)
        {
            email = email.ToLowerInvariant();
            var now = DateTime.UtcNow;
            var otpInfo = new OtpInfo
            {
                Code = otp,
                CreatedTime = now,
                ExpiryTime = now.AddMinutes(expiryMinutes),
                AttemptCount = 0
            };
            _otpDictionary[email] = otpInfo;
            Console.WriteLine($"[StoreOtp] Lưu OTP cho {email}. OTP: {otp}, Thời gian hết hạn: {otpInfo.ExpiryTime}");
        }

        public static bool CanGenerateNewOtp(string email)
        {
            email = email.ToLowerInvariant();
            if (_otpDictionary.TryGetValue(email, out var existingOtp))
            {
                var timeSinceCreation = DateTime.UtcNow - existingOtp.CreatedTime;
                return timeSinceCreation.TotalMinutes >= 1;
            }
            return true;
        }

        public static bool ValidateOtp(string email, string otp)
        {
            email = email.ToLowerInvariant();
            if (!_otpDictionary.TryGetValue(email, out var otpInfo))
            {
                Console.WriteLine($"[ValidateOtp] Không tìm thấy OTP cho {email}");
                return false;
            }

            if (otpInfo.Code != otp.Trim())
            {
                otpInfo.AttemptCount++;
                Console.WriteLine($"[ValidateOtp] Mã OTP không đúng cho {email}. Nhập: {otp}, Đúng: {otpInfo.Code}, Số lần thử: {otpInfo.AttemptCount}");
                return false;
            }

            if (DateTime.UtcNow > otpInfo.ExpiryTime)
            {
                otpInfo.AttemptCount++;
                Console.WriteLine($"[ValidateOtp] OTP hết hạn cho {email}. Nhập: {otp}, Hết hạn lúc: {otpInfo.ExpiryTime}, Hiện tại: {DateTime.UtcNow}");
                return false;
            }

            Console.WriteLine($"[ValidateOtp] OTP hợp lệ cho {email}");
            return true;
        }


        public static int GetAttemptCount(string email)
        {
            email = email.ToLowerInvariant();
            if (_otpDictionary.TryGetValue(email, out var otpInfo))
            {
                return otpInfo.AttemptCount;
            }
            return 0;
        }

        public static void IncrementAttemptCount(string email)
        {
            email = email.ToLowerInvariant();
            if (_otpDictionary.TryGetValue(email, out var otpInfo))
            {
                otpInfo.AttemptCount++;
            }
        }

        public static void RemoveOtp(string email)
        {
            email = email.ToLowerInvariant();
            _otpDictionary.TryRemove(email, out _);
        }

        public static bool HasActiveOtp(string email)
        {
            email = email.ToLowerInvariant();
            return _otpDictionary.TryGetValue(email, out var otpInfo) &&
                   DateTime.UtcNow <= otpInfo.ExpiryTime;
        }

        public static async Task<bool> SendOtpEmail(string email, string otp, IConfiguration configuration)
        {
            email = email.ToLowerInvariant();
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = configuration["EmailSettings:SmtpServer"],
                    Port = int.Parse(configuration["EmailSettings:Port"]),
                    EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]),
                    Credentials = new NetworkCredential(
                        configuration["EmailSettings:SenderEmail"],
                        configuration["EmailSettings:Password"]
                    )
                };

                var message = new MailMessage
                {
                    From = new MailAddress(configuration["EmailSettings:SenderEmail"], configuration["EmailSettings:SenderName"]),
                    Subject = "Mã xác thực đăng ký tài khoản",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Mã xác thực đăng ký tài khoản</h2>
                            <p>Xin chào,</p>
                            <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                            <p>Mã này có hiệu lực trong vòng 5 phút.</p>
                            <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
                            <p>Trân trọng,<br/>Hệ thống đặt lịch khám bệnh</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                message.To.Add(email);
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                return false;
            }
        }

        public static bool MaxAttemptsReached(string email, int maxAttempts = 3)
        {
            email = email.ToLowerInvariant();
            if (_otpDictionary.TryGetValue(email, out var otpInfo))
            {
                Console.WriteLine($"[MaxAttempts] {email} đã thử {otpInfo.AttemptCount} lần, giới hạn là {maxAttempts}");
                return otpInfo.AttemptCount >= maxAttempts;
            }
            return false;
        }

        public static async Task<bool> SendResetPasswordEmail(string email, string otp, IConfiguration configuration)
        {
            email = email.ToLowerInvariant();
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = configuration["EmailSettings:SmtpServer"],
                    Port = int.Parse(configuration["EmailSettings:Port"]),
                    EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]),
                    Credentials = new NetworkCredential(
                        configuration["EmailSettings:SenderEmail"],
                        configuration["EmailSettings:Password"]
                    )
                };

                var message = new MailMessage
                {
                    From = new MailAddress(configuration["EmailSettings:SenderEmail"], configuration["EmailSettings:SenderName"]),
                    Subject = "Mã xác thực đổi mật khẩu",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Mã xác thực đổi mật khẩu</h2>
                            <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                            <p>Mã này có hiệu lực trong vòng 5 phút.</p>
                            <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
                            <p>Trân trọng,<br/>Hệ thống</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                message.To.Add(email);
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                return false;
            }
        }
    }
}
