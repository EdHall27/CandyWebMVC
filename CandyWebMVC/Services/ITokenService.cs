using CandyWebMVC.Models;

namespace CandyWebMVC.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
