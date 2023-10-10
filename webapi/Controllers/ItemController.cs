using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Requests;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Mapping;
using webapi.Contracts.Policies;
using webapi.Contracts.Responses.Item;

namespace webapi.Controllers
{
    [ApiController]
    [Route(ItemEndpoints.BaseUrl)]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ControllerHelper _controllerHelper;

        public ItemController(IItemService itemService, IUserService userService, IMapper mapper, ControllerHelper controllerHelper)
        {
            _itemService = itemService;
            _userService = userService;
            _mapper = mapper;
            _controllerHelper = controllerHelper;
        }

        [Authorize]
        [HttpPost(ItemEndpoints.Create)]
        public async Task<IActionResult> Create([FromBody] CreateItemRequest item, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            return await _controllerHelper.CreateAndRespond(() => _itemService.Create(item, username, cancel), AppMapper.MapToResponse<AddItemResponse>, _mapper);
        }

        [Authorize(Policy = Policies.ItemOwner)]
        [HttpGet(ItemEndpoints.Inactive)]
        public async Task<IActionResult> ListInactive(CancellationToken cancel = default)
        {
            return await ListAll(active: true, cancel);
        }

        [Authorize(Policy = Policies.ItemOwner)]
        [HttpGet(ItemEndpoints.Active)]
        public async Task<IActionResult> ListActive(CancellationToken cancel = default)
        {
            return await ListAll(active: false, cancel);
        }

        [Authorize(Policy = Policies.ItemOwner)]
        [HttpGet(ItemEndpoints.Bidden)]
        public async Task<IActionResult> ListBidden(CancellationToken cancel = default)
        {
            return await ListAll(active: true, cancel);
        }

        [Authorize(Policy = Policies.ItemOwner)]
        [HttpPut(ItemEndpoints.Activate)]
        public async Task<IActionResult> Activate([FromRoute] long id, [FromBody] ActivateItemRequest input, CancellationToken cancel = default)
        {
            try
            {
                var item = await _itemService.Activate(id, input, cancel);

                return Ok(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AuthorizeMultiplePolicies(Policies.Admin, Policies.ItemOwner)]
        [HttpDelete(ItemEndpoints.Delete)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancel = default)
        {
            try
            {
                await _itemService.Delete(id, cancel);
                return NoContent();
            }
            catch (Exception)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }
        }

        [HttpGet(ItemEndpoints.All)]
        public async Task<IActionResult> ListAll(bool active, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            var user = await _userService.GetByUsername(username, cancel);

            if (user is null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            var items = await _itemService.GetItemsOfUserBasedOnStatus(user.Id, active, cancel);

            return Ok(items);
        }

        [HttpGet(ItemEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancel = default)
        {
            var item = await _itemService.GetById(id, cancel);

            return _controllerHelper.CheckNullAndRespond(item);
        }
    }
}
