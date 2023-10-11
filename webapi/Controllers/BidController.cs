using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Mapping;
using webapi.Contracts.Requests;
using webapi.Contracts.Responses.Item;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [Route(UserEndpoints.BaseUrl)]
    public class BidController : Controller
    {
        private readonly IBidService _bidService;
        private readonly IMapper _mapper;
        private readonly ControllerHelper _controllerHelper;

        public BidController(IBidService bidService, IMapper mapper, ControllerHelper controllerHelper)
        {
            _bidService = bidService;
            _mapper = mapper;
            _controllerHelper = controllerHelper;
        }

        [Authorize]
        [HttpPost(BidEndpoints.Create)]
        public async Task<IActionResult> CreateBid([FromBody] AddBidRequest body, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond(() => _bidService.Create(body.ItemId, cancel),
                AppMapper.MapToResponse<AddItemResponse>, _mapper);
        }

        [HttpGet(BidEndpoints.GetItemBids)]
        public async Task<IActionResult> GetItemBids([FromQuery] long itemId, CancellationToken cancel = default)
        {
            var bids = await _bidService.GetItemBids(itemId, cancel);

            return Ok(bids);
        }
    }
}
