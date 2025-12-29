using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using server.Models;

namespace server.Util
{
    public static class JwtUtil
    {
        public class DecodedToken
        {
            public string UserId { get; set; }
            public string UserRole { get; set; }
        }

        public static string GenerateToken(ApplicationUser user, IList<string> roles, int timeExp, IConfiguration _configuration)
        {
            var key = Encoding.UTF8.GetBytes( _configuration["Jwt:SecretKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(timeExp),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }

        public static DecodedToken DecodeToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            var nameIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");

            if (nameIdClaim == null && roleClaim == null) return null;

            string userId = nameIdClaim.Value;
            string userRole = roleClaim.Value;

            return new DecodedToken { UserId = userId, UserRole = userRole };
        }
    }
}