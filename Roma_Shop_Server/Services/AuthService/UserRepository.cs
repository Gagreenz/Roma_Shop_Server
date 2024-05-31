using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Roma_Shop_Server.Dtos.User;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using Roma_Shop_Server.Models.DB;
using Roma_Shop_Server.Services.AccountService.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Roma_Shop_Server.Services.AccountService
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        public UserRepository(ApplicationContext userContext)
        {
            _context = userContext;
        }

        public async Task<ServiceResponse<User>> Login(UserLoginDto userLoginDto)
        {
            ServiceResponse<User> response = new ServiceResponse<User>();

            User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = null;
                response.Message = $"Email {userLoginDto.Email} not found.";

                return response;
            }

            if (!IsPasswordEqual(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.IsSuccess = false;
                response.Data = null;
                response.Message = "Wrong password.";
            }

            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> Register(UserRegisterDto userRegisterDto)
        {
            ServiceResponse<User> response = new ServiceResponse<User>();
            if (await IsUserExist(userRegisterDto.Email))
            {
                response.IsSuccess = false;
                response.Data = null;
                response.Message = "User already exists.";

                return response;
            }

            CreatePasswordHash(userRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            EntityEntry<User> entityEntry = await _context.AddAsync(new User
            {
                Email = userRegisterDto.Email,
                Phone = userRegisterDto.Phone,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            await _context.SaveChangesAsync();

            response.Data = entityEntry.Entity;
            return response;
        }
        public async Task<bool> IsUserExist(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return true;
            }
            return false;
        }
        public async Task<ServiceResponse<User>> GetUserById(string id)
        {
            ServiceResponse<User> response = new ServiceResponse<User>();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = null;
                response.Message = $"User with id:{id} not found.";

                return response;
            }

            response.Data = user;
            return response;
        }
        public async Task SetRefreshToken(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteRefreshToken(JwtSecurityToken jwt)
        {
            var userId = jwt.Claims.First(c => c.Type == "UserId").Value;
            var serviceResponse = await GetUserById(userId);
            if (!serviceResponse.IsSuccess) return false;

            var jwtId = jwt.Claims.First(c => c.Type == "Id").Value;
            var token = _context.RefreshTokens.FirstOrDefault(rt => rt.Id == jwtId);
            if (token == null) return false;

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            return true;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool IsPasswordEqual(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var temp = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return passwordHash.SequenceEqual(temp);
            }
        }

    }
}
