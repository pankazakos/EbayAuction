using contracts.Requests.Bid;
using webapi.Models;
using webapi.Repository.Interfaces;
using webapi.Services.Interfaces;

namespace webapi.Services
{
    public class BidService : IBidService
    {
        private readonly IBidRepository _bidRepository;
        private readonly IUserService _userService;
        private readonly IItemService _itemService;
        private readonly ILogger<BidService> _logger;

        public BidService(IBidRepository repository, IUserService userService, IItemService itemService, ILogger<BidService> logger)
        {
            _bidRepository = repository;
            _userService = userService;
            _itemService = itemService;
            _logger = logger;
        }

        public async Task<Bid> Create(AddBidRequest input, string bidder, CancellationToken cancel = default)
        {
            try
            {
                input.Validate();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }

            var user = await _userService.GetByUsername(bidder, cancel);

            if (user is null)
            {
                _logger.LogWarning("User {Username} does not exist", bidder);
                throw new InvalidOperationException($"Cannot create bid. User {bidder} does not exist");
            }

            var item = await _itemService.GetById(input.ItemId, cancel);

            if (item is null)
            {
                _logger.LogWarning("Item {ItemId} does not exist", input.ItemId);
                throw new ArgumentException($"Cannot create bid. Item {input.ItemId} does not exist");
            }

            if (item.SellerId == user.Id)
            {
                _logger.LogWarning("User {Username} should not make bids for his own items", user.Username);
                throw new InvalidOperationException($"Users cannot make bids for their own items (username: {user.Username})");
            }

            if (item.NumBids > 0 && input.Amount <= item.Currently)
            {
                _logger.LogWarning("Bid cannot be less or equal to current bid. Must be greater than {Amount}", input.Amount);
                throw new ArgumentException($"Bid cannot be less or equal to current bid. Must be greater than {input.Amount}");
            }

            if (item.NumBids == 0 && input.Amount < item.FirstBid)
            {
                _logger.LogWarning("Bid cannot be less than first bid. Must be equal or greater than {Amount}", item.FirstBid);
                throw new ArgumentException($"Bid cannot be less than first bid. Must be equal or greater than {item.FirstBid}");
            }

            var bid = await _bidRepository.Create(input, user, item, cancel);

            _logger.LogInformation("Bid {bidId} created for item {itemId}", bid.BidId, bid.ItemId);

            return bid;
        }

        public async Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default)
        {
            var item = await _itemService.GetById(itemId, cancel);

            if (item is null)
            {
                _logger.LogWarning("Cannot get bids. Item {ItemId} was not found", itemId);
                throw new ArgumentException($"Item {itemId} does not exist");
            }

            var bids = await _bidRepository.GetItemBids(itemId, cancel);

            _logger.LogInformation("Retrieved bids of item {ItemId}", itemId);

            return bids;
        }
    }
}
