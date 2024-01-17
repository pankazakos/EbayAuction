using contracts.Requests.Bid;
using contracts.Responses.bid;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Models;
using webapi.Repository.Interfaces;

namespace webapi.Repository
{
    public class BidRepository : IBidRepository
    {
        private readonly IAuctionContext _dbContext;

        public BidRepository(IAuctionContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Bid> Create(AddBidRequest input, User bidder, Item item, CancellationToken cancel = default)
        {
            var bid = new Bid
            {
                Time = DateTime.Now,
                Amount = input.Amount,
                ItemId = input.ItemId,
                BidderId = bidder.Id,
                Bidder = bidder,
                Item = item,
            };

            item.Currently = bid.Amount;

            item.NumBids++;

            _dbContext.Bids.Add(bid);

            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Bids.Remove(bid);
            }

            return bid;
        }

        public async Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default)
        {
            return await _dbContext.Bids.Where(b => b.ItemId == itemId).ToListAsync(cancel);
        }

        public async Task<IEnumerable<Bid>> GetUserBids(int userId, CancellationToken cancel = default)
        {
            return await _dbContext.Bids.Where(b => b.BidderId == userId).ToListAsync(cancel);
        }

        public async Task<IEnumerable<ExtendedBidInfo>> GetExtendedInfoUserBids(int userId, CancellationToken cancel = default)
        {
            var extendedBids = await _dbContext.Bids
                .Where(bid => bid.BidderId == userId)
                .Select(bid => new ExtendedBidInfo
                {
                    BidId = bid.BidId,
                    Time = bid.Time,
                    Amount = bid.Amount,
                    ItemId = bid.ItemId,
                    BidderId = bid.BidderId,
                    Seller = bid.Item.Seller.Username,
                    ItemTitle = bid.Item.Name,
                    AuctionStatus = DateTime.Now < bid.Item.Ends ? AuctionStatusType.Active : AuctionStatusType.Expired
                }).ToListAsync(cancel);

            return extendedBids;
        }

        public Task<Bid?> GetLastBidOfUser(int userId, long itemId, CancellationToken cancel = default)
        {
            return _dbContext.Bids.Where(b => b.BidderId == userId && b.ItemId == itemId)
                                .OrderByDescending(b => b.Time).FirstOrDefaultAsync(cancel);
        }
    }
}
