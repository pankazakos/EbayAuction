using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Repository
{
    public interface IItemRepository
    {
        public Task<Item> Create(CreateItemRequest item, CancellationToken cancel = default);

        public Task<Item?> GetById(long id, CancellationToken cancel = default);

        public Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default);

        public Task<Item> Activate(long id, DateTime expiration, CancellationToken cancel = default);
    }
}
