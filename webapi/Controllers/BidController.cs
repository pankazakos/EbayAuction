using AutoMapper;
using contracts.Endpoints;
using contracts.Requests.Bid;
using contracts.Responses.bid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services;
using webapi.Utilities.ControllerUtils;
using webapi.Utilities.MappingUtils;

namespace webapi.Controllers
{
    [Route(BidEndpoints.BaseUrl)]
    public class BidController : Controller
    {
        private readonly IBidService _bidService;
        private readonly IMapper _mapper;
        private readonly IControllerHelper _controllerHelper;

        public BidController(IBidService bidService, IMapper mapper, IControllerHelper controllerHelper)
        {
            _bidService = bidService;
            _mapper = mapper;
            _controllerHelper = controllerHelper;
        }


        [Authorize]
        [HttpPost(BidEndpoints.Create)]
        public async Task<IActionResult> CreateBid([FromBody] AddBidRequest body, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond<Bid, AddBidResponse>(
                () => _bidService.Create(body.ItemId, cancel), _mapper);
        }


        [HttpGet(BidEndpoints.GetItemBids)]
        public async Task<IActionResult> GetItemBids([FromQuery] long itemId, CancellationToken cancel = default)
        {
            var bids = await _bidService.GetItemBids(itemId, cancel);

            var mappedBids = bids.Select(bid => bid.MapToResponse<AddBidResponse>(_mapper)).ToList();

            return Ok(mappedBids);
        }
    }
}
