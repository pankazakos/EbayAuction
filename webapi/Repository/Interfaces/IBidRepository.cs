using contracts.Requests.Bid;
using webapi.Models;

namespace webapi.Repository.Interfaces
{
    public interface IBidRepository
    {
        public Task<Bid> Create(AddBidRequest input, User bidder, Item item, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);
    }
}
