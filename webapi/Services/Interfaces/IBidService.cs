using contracts.Requests.Bid;
using webapi.Models;

namespace webapi.Services.Interfaces
{
    public interface IBidService
    {
        public Task<Bid> Create(AddBidRequest input, string bidder, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);
    }
}
