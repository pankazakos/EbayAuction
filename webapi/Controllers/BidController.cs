using AutoMapper;
using contracts.Endpoints;
using contracts.Requests.Bid;
using contracts.Responses.bid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services.Interfaces;
using webapi.Utilities.ControllerUtils;

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
        public async Task<IActionResult> CreateBid([FromBody] AddBidRequest request, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            return await _controllerHelper.CreateAndRespond<Bid, BasicBidResponse>(
                () => _bidService.Create(request, username, cancel), _mapper);
        }


        [HttpGet(BidEndpoints.GetItemBids)]
        public async Task<IActionResult> GetItemBids([FromQuery] long itemId, CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllAndRespond<Bid, BasicBidResponse>(
                () => _bidService.GetItemBids(itemId, cancel), _mapper);
        }


        [Authorize]
        [HttpGet(BidEndpoints.GetUserBids)]
        public async Task<IActionResult> GetUserBids([FromQuery] GetBidsOrderOptions? orderOptions, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            return await _controllerHelper.GetAllAndRespond<Bid, BasicBidResponse>(
                               () => _bidService.GetUserBids(orderOptions, username, cancel), _mapper);
        }


        [Authorize]
        [HttpGet(BidEndpoints.GetFullInfoUserBids)]
        public async Task<IActionResult> GetExtendedInfoUserBids([FromQuery] GetBidsOrderOptions? orderOptions, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            try
            {
                var bids = await _bidService.GetExtendedInfoUserBids(orderOptions, username, cancel);
                return Ok(bids);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpGet(BidEndpoints.GetLastBidOfUser)]
        public async Task<IActionResult> GetLastBidOfUser([FromQuery] long itemId, CancellationToken cancel = default)
        {
            var username = _controllerHelper.UsernameClaim;

            return await _controllerHelper.GetAndRespond<Bid?, BasicBidResponse>(
                               () => _bidService.GetLastBidOfUser(username, itemId, cancel), _mapper);
        }
    }
}
