using System.Globalization;
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
            if (item is null)
            {
                throw new ArgumentException("Invalid data.");
            }

            // Get or create categories


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

        public async Task<Item> Activate(long id, ActivateItemRequest input, CancellationToken cancel = default)
        {
            const string format = "yyyy-MM-dd HH:mm";

            var dtInput = input.Expiration.ToString(CultureInfo.InvariantCulture);

            if (!DateTime.TryParseExact(dtInput, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExpiration))
            {
                throw new ArgumentException("DateTime format is not correct. Correct format is: \"yyyy-MM-dd HH:mm\" ");
            }

            var now = DateTime.Now.ToString(format);

            DateTime.TryParse(now, out var dtNow);

            if (dtExpiration <= dtNow)
            {
                throw new ArgumentException("Ends datetime cannot be set to a datetime earlier than the current datetime.");
            }

            return await _itemRepository.Activate(id, input.Expiration, cancel);
        }
    }
}
