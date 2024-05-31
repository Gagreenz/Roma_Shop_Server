using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roma_Shop_Server.Data;
using Roma_Shop_Server.Dtos.User;
using Roma_Shop_Server.Models;
using Roma_Shop_Server.Models.Data;
using Roma_Shop_Server.Services.AccountService.Interfaces;
using Roma_Shop_Server.Services.TokenService.Interfaces;

namespace Roma_Shop_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AccountController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var serviceResponse = await _userRepository.Login(userLoginDto);
            if (!serviceResponse.IsSuccess)
            {
                return BadRequest(serviceResponse.Message);
            }
            var user = serviceResponse.Data!;
            if (user == null) return BadRequest("Server error");

            await setRefreshToken(user);

            AuthResponse response = new AuthResponse()
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
            };

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var serviceResponse = await _userRepository.Register(userRegisterDto);

            if (!serviceResponse.IsSuccess)
            {
                return BadRequest(serviceResponse.Message);
            }

            return Ok();
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (refreshToken == null) return BadRequest("Missing RefreshToken");
            var jwt = _tokenService.GetJwt(refreshToken);
            if (!(await _userRepository.DeleteRefreshToken(jwt))) return BadRequest("Missing RefreshToken");

            return Ok();
        }

        [HttpPost("ResetTokens")]
        public async Task<ActionResult<AuthResponse>> ResetTokens()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (refreshToken == null) return BadRequest("Missing RefreshToken");

            var jwt = _tokenService.GetJwt(refreshToken);
            var userId = jwt.Claims.First(c => c.Type == ArnegJWTClaims.UserId).Value;
            var serviceResponse = await _userRepository.GetUserById(userId);

            var user = serviceResponse.Data!;
            if (user == null) return BadRequest("Server error");

            if (!serviceResponse.IsSuccess) return BadRequest(serviceResponse!.Message);
            if (!_tokenService.VerifyRefreshToken(user, refreshToken)) return BadRequest("Missing RefreshToken");

            await _userRepository.DeleteRefreshToken(jwt);
            await setRefreshToken(user);

            AuthResponse response = new AuthResponse()
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
            };
            return Ok(response);
        }
        private async Task setRefreshToken(User user)
        {
            RefreshToken refreshToken = _tokenService.GenerateRefreshToken(user);
            await _userRepository.SetRefreshToken(refreshToken);

            var AccessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = refreshToken.ExpiredAt,
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Append("RefreshToken", refreshToken.Token, AccessCookieOptions);
        }
    }
}
