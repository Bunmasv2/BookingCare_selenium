using server.Models;

namespace server.Services
{
    public interface IAuth
    {
        Task<string> GetRefreshToken(int userId);
        bool VerifyToken(string token);
        Task SaveRefreshToken (ApplicationUser user, string refreshToken);
    }
}
