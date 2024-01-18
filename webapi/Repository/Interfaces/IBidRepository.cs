using contracts.Requests.Bid;
using contracts.Responses.bid;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Repository.Interfaces
{
    public interface IBidRepository
    {
        public Task<Bid> Create(AddBidRequest input, User bidder, Item item, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetUserBids(int userId, CancellationToken cancel = default);

        public Task<IEnumerable<ExtendedBidInfo>> GetExtendedInfoUserBids([FromRoute] GetBidsOrderOptions? orderOptions, 
            int userId, CancellationToken cancel = default);

        public Task<Bid?> GetLastBidOfUser(int userId, long itemId, CancellationToken cancel = default);
    }
}
