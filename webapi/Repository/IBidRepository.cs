using webapi.Models;

namespace webapi.Repository
{
    public interface IBidRepository
    {
        public Task<Bid> Create(long itemId);
    }
}
