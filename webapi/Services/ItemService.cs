using webapi.Contracts.Requests;
using webapi.Models;
using webapi.Repository;

namespace webapi.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<Item> Create(CreateItemRequest item, CancellationToken cancel)
        {
            if (item == null)
            {
                throw new ArgumentException("Invalid data.");
            }

            return await _itemRepository.Create(item, cancel);
        }

        public async Task<Item?> GetById(long id, CancellationToken cancel)
        {
            return await _itemRepository.GetById(id, cancel);
        }


        public async Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel)
        {
            return await _itemRepository.GetItemsOfUserBasedOnStatus(userId, active, cancel);
        }
    }
}
