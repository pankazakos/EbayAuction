using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Services
{
    public interface IItemService
    {
        public Task<Item> Create(CreateItemRequest item, CancellationToken cancel = default);

        public Task<Item?> GetById(long id, CancellationToken cancel = default);

        public Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default);

        public Task<Item> Activate(long id, ActivateItemRequest input, CancellationToken cancel = default);

        public Task Delete(long id, CancellationToken cancel = default);
    }
}
