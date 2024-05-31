using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using System.IdentityModel.Tokens.Jwt;

namespace Roma_Shop_Server.Services.TokenService.Interfaces
{
    public interface ITokenService
    {
        public string GenerateAccessToken(User user);
        public RefreshToken GenerateRefreshToken(User user);
        public JwtSecurityToken GetJwt(string token);
        public bool VerifyRefreshToken(User user, string refreshToken);
    }
}
