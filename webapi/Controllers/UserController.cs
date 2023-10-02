using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webapi.Contracts.Requests;
using webapi.Services;
using webapi.Utilities;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Mapping;
using webapi.Contracts.Policies;
using webapi.Contracts.Responses;

namespace webapi.Controllers
{
    [Route(UserEndpoints.BaseUrl)]
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

        [Authorize(Policy = Policies.Admin)]
        [HttpGet(UserEndpoints.All)]
        public async Task<IActionResult> ListAll(CancellationToken cancel = default)
        {
            var users = await _userService.GetAll(cancel);

            return Ok(users);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet(UserEndpoints.Usernames)]
        public async Task<IActionResult> ListUsernames(CancellationToken cancel = default)
        {
            var usernames = await _userService.GetAllUsernames(cancel);

            return Ok(usernames);
        }

        [Authorize]
        [HttpGet(UserEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancel = default)
        {
            var user = await _userService.GetById(id, cancel);

            if (user is null)
            {
                return _controllerHelper.CheckNullAndRespond(user);
            }

            var username = _controllerHelper.UsernameClaim;

            if (username != user.Username && !user.IsSuperuser)
            {
                return Forbid();
            }

            return Ok(user);
        }

        [HttpPost(UserEndpoints.Create)]
        public async Task<IActionResult> Create([FromBody] UserCredentialsRequest input, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond(() => _userService.Create(input, cancel), UserMapper.MapToCreateUserResponse);
        }

        [HttpPost(UserEndpoints.Login)]
        public async Task<IActionResult> Login([FromBody] UserCredentialsRequest input, CancellationToken cancel = default)
        {
            var user = await _userService.GetByUsername(input.Username);

            if (user is null)
            {
                return _controllerHelper.CheckNullAndRespond(user);
            }

            if (!PasswordHelper.VerifyPassword(input.Password, user.PasswordHash, user.PasswordSalt))

                return BadRequest("Password is incorrect.");


            return Ok(new LoginUserResponse
            {
                AccesToken = new JwtHelper(_configuration).GenerateAccessToken(user.Username, user.IsSuperuser)
            });
        }
    }
}
