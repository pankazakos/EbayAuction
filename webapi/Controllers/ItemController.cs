using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Requests;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly ControllerHelper _controllerHelper;

        public ItemController(IItemService itemService, IUserService userService, ControllerHelper controllerHelper)
        {
            _itemService = itemService;
            _userService = userService;
            _controllerHelper = controllerHelper;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateItemRequest item, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond(() => _itemService.Create(item, cancel));
        }

        [Authorize]
        [HttpGet("ListItems")]
        public async Task<IActionResult> ListItems(bool active, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                return _controllerHelper.NotFoundRespond<User>();
            }

            var items = await _itemService.GetItemsOfUserBasedOnStatus(user.Id, active, cancel);

            return _controllerHelper.CheckCountAndRespond(items);
        }

        [Authorize]
        [HttpGet("ListInactive")]
        public async Task<IActionResult> ListInactive(CancellationToken cancel = default)
        {
            return await ListItems(active: true, cancel);
        }

        [Authorize]
        [HttpGet("ListActive")]
        public async Task<IActionResult> ListActive(CancellationToken cancel = default)
        {
            return await ListItems(active: false, cancel);
        }

        [Authorize]
        [HttpGet("ListBidden")]
        public async Task<IActionResult> ListBidden(CancellationToken cancel = default)
        {
            return await ListItems(active: true, cancel);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancel = default)
        {
            var item = await _itemService.GetById(id, cancel);

            return _controllerHelper.CheckNullAndRespond(item);
        }
    }
}
