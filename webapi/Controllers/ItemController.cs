using AutoMapper;
using contracts.Endpoints;
using contracts.Policies;
using contracts.Requests.Item;
using contracts.Responses.Category;
using contracts.Responses.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using webapi.Models;
using webapi.Services.Interfaces;
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
        public async Task<IActionResult> Create([FromForm] string itemJson, [FromForm] IFormFile? image, CancellationToken cancel = default)
        {
            var item = JsonConvert.DeserializeObject<AddItemRequest>(itemJson);
            if (item == null)
            {
                return BadRequest("Invalid item data");
            }

            var username = _controllerHelper.UsernameClaim;

            if (image is null)
            {
                return await _controllerHelper.CreateAndRespond<Item, AddItemResponse>(
                    () => _itemService.Create(item, username, cancel: cancel), _mapper);
            }

            return await _controllerHelper.CreateAndRespond<Item, AddItemResponse>(
                () => _itemService.Create(item, username, image, cancel), _mapper);
        }


        private async Task<IActionResult> ListMyItems(bool active, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            var user = await _userService.GetByUsername(username, cancel);

            if (user is null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            return await _controllerHelper.GetAllAndRespond<Item, BasicItemResponse>(
                () => _itemService.GetItemsOfUserBasedOnStatus(user.Id, active, cancel), _mapper);
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
            var username = _controllerHelper.UsernameClaim;

            var user = await _userService.GetByUsername(username, cancel);

            if (user is null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            return await _controllerHelper.GetAllAndRespond<Item, BasicItemResponse>(
                () => _itemService.GetBidden(user.Id, cancel), _mapper);
        }


        [Authorize(Policy = Policies.ItemOwner)]
        [HttpPut(ItemEndpoints.Activate)]
        public async Task<IActionResult> Activate([FromRoute] long id, [FromBody] PublishItemRequest input, CancellationToken cancel = default)
        {
            try
            {
                var item = await _itemService.Activate(id, input, cancel);

                return Ok(item.MapToResponse<BasicItemResponse>(_mapper));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = Policies.ItemOwner)]
        [HttpPut(ItemEndpoints.Edit)]
        public async Task<IActionResult> Edit([FromRoute] long id, [FromForm] string itemJson, [FromForm] IFormFile? image, CancellationToken cancel = default)
        {
            EditItemRequest? itemData;
            try
            {
                itemData = JsonConvert.DeserializeObject<EditItemRequest>(itemJson);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (itemData == null)
            {
                return BadRequest("Invalid item data");
            }

            try
            {
                if (image is null)
                {
                    var item = await _itemService.Edit(id, itemData, cancel: cancel);
                    return Ok(item.MapToResponse<BasicItemResponse>(_mapper));
                }

                var itemWithImage = await _itemService.Edit(id, itemData, image, cancel);
                return Ok(itemWithImage.MapToResponse<BasicItemResponse>(_mapper));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
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


        [HttpGet(ItemEndpoints.Search)]
        public async Task<IActionResult> Search([FromQuery] ItemSearchQuery query, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetPagedAndRespond<Item, BasicItemResponse>(
                () => _itemService.Search(query, cancel), query.Page, query.Limit, _mapper);
        }


        [HttpGet(ItemEndpoints.GetImage)]
        public async Task<IActionResult> Image([FromRoute] string guid, CancellationToken cancel = default)
        {
            try
            {
                var content = await _itemService.GetImage(guid, cancel);

                var stream = await content.ReadAsStreamAsync(cancel);

                var mediaType = content.Headers.ContentType!.MediaType!;

                return new FileStreamResult(stream, mediaType);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet(ItemEndpoints.Categories)]
        public async Task<IActionResult> Categories([FromRoute] long id, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllAndRespond<Category, BasicCategoryResponse>(
                               () => _itemService.GetCategories(id, cancel), _mapper);
        }


        [HttpGet(ItemEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAndRespond<Item?, BasicItemResponse>(
                () => _itemService.GetById(id, cancel), _mapper);
        }
    }
}
