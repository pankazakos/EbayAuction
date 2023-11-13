using AutoMapper;
using contracts.Endpoints;
using contracts.Policies;
using contracts.Requests.Item;
using contracts.Responses;
using contracts.Responses.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using webapi.Models;
using webapi.Services;
using webapi.Utilities.AuthorizationUtils.PolicyUtils;
using webapi.Utilities.ControllerUtils;
using webapi.Utilities.MappingUtils;

namespace webapi.Controllers
{
    [ApiController]
    [Route(ItemEndpoints.BaseUrl)]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IControllerHelper _controllerHelper;

        public ItemController(IItemService itemService, IUserService userService, IMapper mapper, IControllerHelper controllerHelper)
        {
            _itemService = itemService;
            _userService = userService;
            _mapper = mapper;
            _controllerHelper = controllerHelper;
        }


        [Authorize]
        [HttpPost(ItemEndpoints.Create)]
        public async Task<IActionResult> Create([FromForm] string itemJson, [FromForm] IFormFile image, CancellationToken cancel = default)
        {
            var item = JsonConvert.DeserializeObject<AddItemRequest>(itemJson);
            if (item == null)
            {
                return BadRequest("Invalid item data");
            }

            var username = _controllerHelper.UsernameClaim;

            if (!Request.HasFormContentType)
            {
                return await _controllerHelper.CreateAndRespond<Item, AddItemResponse>(
                    () => _itemService.Create(item, username, cancel: cancel), _mapper);
            }

            return await _controllerHelper.CreateAndRespond<Item, AddItemResponse>(
                () => _itemService.Create(item, username, image, cancel), _mapper);
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

            foreach (var item in items)
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
        public async Task<IActionResult> ListAllPaged([FromQuery] int page = 1, [FromQuery] int limit = 10, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllPagedAndRespond<Item, BasicItemResponse>(
                () => _itemService.GetAllPaged(page, limit, cancel), page, limit, _mapper);
        }


        [HttpGet(ItemEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAndRespond<Item?, PublishedItemResponse>(
                () => _itemService.GetById(id, cancel), _mapper);
        }
    }
}
