using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Mapping;
using webapi.Contracts.Policies;
using webapi.Contracts.Responses.Item;
using webapi.Contracts.Requests.Item;
using webapi.Contracts.Responses;
using webapi.Utilities.AuthorizationUtils.PolicyUtils;
using webapi.Utilities.ControllerUtils;

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
        public async Task<IActionResult> Create([FromBody] AddItemRequest item, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            return await _controllerHelper.CreateAndRespond(() => _itemService.Create(item, username, cancel), AppMapper.MapToResponse<AddItemResponse>, _mapper);
        }


        [Authorize]
        [HttpGet(ItemEndpoints.MyItems)]
        public async Task<IActionResult> ListMyItems(bool active, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            var user = await _userService.GetByUsername(username, cancel);

            if (user is null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            var items = await _itemService.GetItemsOfUserBasedOnStatus(user.Id, active, cancel);

            List<IEntityResponse> mappedItems = new();

            foreach(var item in items)
            {
                mappedItems.Add(item.MapToResponse<AddItemResponse>(_mapper));
            }

            return Ok(mappedItems);
        }


        [Authorize]
        [HttpGet(ItemEndpoints.Inactive)]
        public async Task<IActionResult> ListInactive(CancellationToken cancel = default)
        {
            return await ListMyItems(false, cancel);
        }


        [Authorize]
        [HttpGet(ItemEndpoints.Active)]
        public async Task<IActionResult> ListActive(CancellationToken cancel = default)
        {
            return await ListMyItems(true, cancel);
        }


        [Authorize]
        [HttpGet(ItemEndpoints.Bidden)]
        public async Task<IActionResult> ListBidden(CancellationToken cancel = default)
        {
            return await ListMyItems(true, cancel);
        }


        [Authorize(Policy = Policies.ItemOwner)]
        [HttpPut(ItemEndpoints.Activate)]
        public async Task<IActionResult> Activate([FromRoute] long id, [FromBody] PublishItemRequest input, CancellationToken cancel = default)
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
            return await _controllerHelper.DeleteAndRespond<Item>(() => _itemService.Delete(id, cancel));
        }


        [HttpGet(ItemEndpoints.All)]
        public async Task<IActionResult> All(CancellationToken cancel = default)
        {
            var items = await _itemService.ListAll(cancel);

            return Ok(items);
        }


        [HttpGet(ItemEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAndRespond(
                () => _itemService.GetById(id, cancel),
                (item, mapper) => item!.MapToResponse<PublishedItemResponse>(mapper), _mapper);
        }
    }
}
