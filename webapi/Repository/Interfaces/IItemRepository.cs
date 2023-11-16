using contracts.Requests.Item;
using webapi.Models;

namespace webapi.Repository.Interfaces
{
    public interface IItemRepository
    {
        public Task<Item> Create(AddItemRequest item, User seller, IFormFile? postedFile = null, string? fileName = null, CancellationToken cancel = default);

        public Task<(IEnumerable<Item>, int)> Search(ItemSearchQuery query, CancellationToken cancel = default);

        public Task<Item?> GetById(long id, CancellationToken cancel = default);

        public Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default);

        public Task<Item> Activate(long id, DateTime expiration, CancellationToken cancel = default);

        public Task Delete(Item item, CancellationToken cancel = default);
    }
}
