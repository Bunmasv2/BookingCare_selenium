using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using server.Middleware;

public class AuthToken
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthToken> _logger;

    public AuthToken(RequestDelegate next, ILogger<AuthToken> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/specialties") ||
            context.Request.Path.StartsWithSegments("/api/auth/login") ||
            context.Request.Path.StartsWithSegments("/api/auth/Signin") ||
            context.Request.Path.StartsWithSegments("/api/services") ||
            context.Request.Path.StartsWithSegments("/api/other-public-api") ||
            context.Request.Path.StartsWithSegments("/api/doctors") ||
            context.Request.Path.StartsWithSegments("/api/appointments"))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        Console.WriteLine(token);
        if (string.IsNullOrEmpty(token))
        {
            throw new ErrorHandlingException(401, "Please log in to continue!");
        }

        AttachUserToContext(context, token);
        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            Console.WriteLine("AttachUserToContext");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("uX9#2fB!rT7z$KpV@8dG%qL*eJ4mW!sN^ZbC@1yH");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            // var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
            var userRole = jwtToken.Claims.First(x => x.Type == "role").Value;

            // context.Items["User"] = userId;
            context.Items["Email"] = userEmail;
            context.Items["Role"] = userRole;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Token invalid: {ex.Message}");
            throw new ErrorHandlingException(401, "Your session is expired!");
        }
    }
}
