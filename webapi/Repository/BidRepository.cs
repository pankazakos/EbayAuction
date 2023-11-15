using webapi.Models;
using webapi.Repository.Interfaces;

namespace webapi.Repository
{
    public class BidRepository : IBidRepository
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
