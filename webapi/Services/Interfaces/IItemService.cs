using contracts.Requests.Item;
using webapi.Models;

namespace webapi.Services.Interfaces
{
    public interface IItemService
    {
        public Task<Item> Create(AddItemRequest item, string username, IFormFile? postedFile = null, CancellationToken cancel = default);

        public Task<(IEnumerable<Item>, int)> Search(ItemSearchQuery query, CancellationToken cancel = default);

        public Task<Item?> GetById(long id, CancellationToken cancel = default);

        public Task<IEnumerable<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default);

        public Task<byte[]> GetImage(string guid, CancellationToken cancel = default);

        public Task<Item> Activate(long id, PublishItemRequest input, CancellationToken cancel = default);

        public Task Delete(long id, CancellationToken cancel = default);
    }
}
