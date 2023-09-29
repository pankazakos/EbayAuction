using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Repository
{
    public interface IItemRepository
    {
        public Task<Item> Create(CreateItemRequest item, CancellationToken cancel);
        public Task<Item?> GetById(long id, CancellationToken token);
        public Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken token);
    }
}
