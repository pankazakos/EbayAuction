using webapi.Models;

namespace webapi.Services
{
    public class BidService : IBidService
    {
        public Task<Bid> Create(long itemId, CancellationToken cancel = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default)
        {
            throw new NotImplementedException();
        }
    }
}
