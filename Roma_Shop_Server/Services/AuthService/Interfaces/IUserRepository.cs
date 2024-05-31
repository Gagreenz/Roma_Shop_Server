using Roma_Shop_Server.Dtos.User;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using System.IdentityModel.Tokens.Jwt;

namespace Roma_Shop_Server.Services.AccountService.Interfaces
{
    public interface IUserRepository
    {
        Task<ServiceResponse<User>> Register(UserRegisterDto userRegisterDto);
        Task<ServiceResponse<User>> Login(UserLoginDto userLoginDto);
        Task<ServiceResponse<User>> GetUserById(string id);
        Task SetRefreshToken(RefreshToken refreshToken);
        Task<bool> DeleteRefreshToken(JwtSecurityToken refreshToken);
        Task<bool> IsUserExist(string login);

    }
}