﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webapi.Services;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Mapping;
using webapi.Contracts.Policies;
using webapi.Contracts.Responses.Other;
using webapi.Contracts.Responses.User;
using webapi.Models;
using webapi.Contracts.Requests.User;
using webapi.Utilities.ControllerUtils;
using webapi.Utilities.AuthorizationUtils.PasswordUtils;
using webapi.Utilities.AuthorizationUtils.PolicyUtils;

namespace webapi.Controllers
{
    [Route(UserEndpoints.BaseUrl)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IControllerHelper _controllerHelper;

        public UserController(IUserService service, IConfiguration configuration, IMapper mapper, IControllerHelper controllerHelper)
        {
            _userService = service;
            _configuration = configuration;
            _mapper = mapper;
            _controllerHelper = controllerHelper;
        }


        [Authorize(Policy = Policies.Admin)]
        [HttpGet(UserEndpoints.All)]
        public async Task<IActionResult> ListAll(CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllAndRespond<User, BasicUserResponse>(() => _userService.GetAll(cancel), _mapper);
        }


        [Authorize(Policy = Policies.Admin)]
        [HttpGet(UserEndpoints.Usernames)]
        public async Task<IActionResult> ListUsernames(CancellationToken cancel = default)
        {
            var usernames = await _userService.GetAllUsernames(cancel);

            return Ok(usernames);
        }


        [AuthorizeMultiplePolicies(Policies.Admin, Policies.SelfUser)]
        [HttpGet(UserEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAndRespond(
                () => _userService.GetById(id, cancel),
                (user, mapper) => user!.MapToResponse<BasicUserResponse>(mapper), _mapper);
        }


        [AuthorizeMultiplePolicies(Policies.Admin, Policies.SelfUser)]
        [HttpGet(UserEndpoints.GetByUsername)]
        public async Task<IActionResult> GetByUsername([FromRoute] string username, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAndRespond(() => _userService.GetByUsername(username, cancel),
               (user, mapper) => user!.MapToResponse<BasicUserResponse>(mapper), _mapper);
        }


        [Authorize(Policy = Policies.Admin)]
        [HttpDelete(UserEndpoints.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancel = default)
        {
            return await _controllerHelper.DeleteAndRespond<User>(() => _userService.Delete(id, cancel));
        }


        [HttpPost(UserEndpoints.Create)]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequest input, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond(() => _userService.Create(input, cancel),
                AppMapper.MapToResponse<RegisterUserResponse>, _mapper);
        }


        [HttpPost(UserEndpoints.Login)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest input, CancellationToken cancel = default)
        {
            var user = await _userService.GetByUsername(input.Username, cancel);

            if (user is null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            if (!PasswordHelper.VerifyPassword(input.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect.");

            }

            var jwt = new JwtHelper(_configuration).GenerateAccessToken(user.Username, user.IsSuperuser);

            try
            {
                await _userService.UpdateLastLogin(input.Username, cancel);
            }
            catch (InvalidOperationException)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            return Ok(new LoginUserResponse
            {
                AccessToken = jwt
            });
        }
    }
}
