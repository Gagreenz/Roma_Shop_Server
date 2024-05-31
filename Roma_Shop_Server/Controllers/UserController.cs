using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roma_Shop_Server.Dtos.User;
using Roma_Shop_Server.Services.AccountService.Interfaces;
using System.Security.Claims;

namespace Roma_Shop_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IHttpContextAccessor _accessor;
        IUserRepository _userRepository;
        public UserController(
            IHttpContextAccessor accessor,
            IUserRepository userRepository

        )
        {
            _accessor = accessor;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var id = _accessor.HttpContext.User.FindFirstValue("UserId");
            var serviceResponse = await _userRepository.GetUserById(id);

            if (!serviceResponse.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, serviceResponse.Message);
            }

            var user = serviceResponse.Data;


            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone
            };

            return Ok(userViewModel);
        }

        [HttpGet("GetUserId")]
        [Authorize]
        public IActionResult GetUserId()
        {
            var id = _accessor.HttpContext.User.FindFirstValue("UserId");
            return Ok(id);
        }
    }
}
