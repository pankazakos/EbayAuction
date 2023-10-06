using webapi.Models;

namespace webapi.Services
{
    public interface IBidService
    {
        public Task<Bid> Create(long itemId);
    }
}
