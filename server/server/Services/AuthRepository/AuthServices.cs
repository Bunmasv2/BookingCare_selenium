using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Models;

namespace server.Services
{
    public class AuthServices : IAuth
    {
        private readonly ClinicManagementContext _context;
        private readonly IMapper _mapper;

        public AuthServices(ClinicManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> GetRefreshToken(int userId)
        {
            ApplicationUser user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            return user?.RefreshToken ?? "";
        }

        public bool VerifyToken(string token)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtToken = jwtHandler.ReadJwtToken(token);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://127.0.0.1:5140",
                    ValidAudience = "http://127.0.0.1:3000",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MộtPassphraseDàiÍtNhất32KýTự1234567890"))
                };
                var claimsPrincipal = jwtHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                // Token hợp lệ
                var decodedToken = (JwtSecurityToken)validatedToken;
                // Sử dụng decodedToken
                return true;
            }
            catch (SecurityTokenException ex)
            {
                // Token không hợp lệ hoặc hết hạn
                // Console.WriteLine("Token không hợp lệ hoặc hết hạn: " + ex.Message);
                // context.Response.StatusCode = 401;
                // context.Response.ContentType = "application/json";
                // await context.Response.WriteAsync(JsonSerializer.Serialize(new { ErrorMessage = "Token không hợp lệ hoặc đã hết hạn." }));
                return false;
            }                
        }

        public async Task SaveRefreshToken(ApplicationUser user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
