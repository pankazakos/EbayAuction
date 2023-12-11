using contracts.Requests.Bid;
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
    }
}
