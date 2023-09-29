using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webapi.Contracts.Requests;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ControllerHelper _controllerHelper;

        public UserController(IUserService service, IConfiguration configuration, ControllerHelper controllerHelper)
        {
            _userService = service;
            _configuration = configuration;
            _controllerHelper = controllerHelper;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("ListAll")]
        public async Task<IActionResult> ListAll(CancellationToken cancel = default)
        {
            var users = await _userService.GetAll(cancel);

            return _controllerHelper.CheckCountAndRespond(users);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("ListUsernames")]
        public async Task<IActionResult> ListUsernames(CancellationToken cancel = default)
        {
            var usernames = await _userService.GetAllUsernames(cancel);

            return _controllerHelper.CheckCountAndRespond(usernames);
        }

        [Authorize]
        [HttpGet("Get/{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancel = default)
        {
            var user = await _userService.GetById(id, cancel);

            if(user == null)
            {
                return _controllerHelper.CheckNullAndRespond(user);
            }

            var username = _controllerHelper.UsernameClaim;

            if(username != user.Username && !user.IsSuperuser)
            {
                return Forbid();
            }

            return Ok(user);

        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest input, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond(() => _userService.Create(input, cancel));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] CreateUserRequest input, CancellationToken cancel = default)
        {
            var user = await _userService.GetByUsername(input.Username);

            if (user == null)
            {
                return _controllerHelper.CheckNullAndRespond(user);
            }

            if (PasswordHelper.VerifyPassword(input.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Ok(new { AccessToken = new JwtHelper(_configuration).GenerateAccessToken(user.Username, user.IsSuperuser)});
            }

            return BadRequest("Password is incorrect.");
        }
    }
}
