using webapi.Contracts.Requests.Item;
using webapi.Models;

namespace webapi.Repository
{
    public interface IItemRepository
    {
        public Task<Item> Create(AddItemRequest item, User seller, CancellationToken cancel = default);

        public Task<IEnumerable<Item>> ListAll(CancellationToken cancel = default);

        public Task<Item?> GetById(long id, CancellationToken cancel = default);

        public Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default);

        public Task<Item> Activate(long id, DateTime expiration, CancellationToken cancel = default);

        public Task Delete(Item item, CancellationToken cancel = default);
    }
}
