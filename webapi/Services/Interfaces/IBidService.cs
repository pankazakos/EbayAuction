using contracts.Requests.Bid;
using contracts.Responses.bid;
using webapi.Models;

namespace webapi.Services.Interfaces
{
    public interface IBidService
    {
        public Task<Bid> Create(AddBidRequest input, string bidder, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetUserBids(GetBidsOrderOptions? orderOptions, string username, CancellationToken cancel = default);

        public Task<IEnumerable<ExtendedBidInfo>> GetExtendedInfoUserBids(GetBidsOrderOptions? orderOptions, string username, CancellationToken cancel = default);

        public Task<Bid?> GetLastBidOfUser(string username, long itemId, CancellationToken cancel = default);
    }
}
