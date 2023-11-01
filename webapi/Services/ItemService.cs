using System.Globalization;
using webapi.Contracts.Requests.Item;
using webapi.Models;
using webapi.Repository;

namespace webapi.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ItemService> _logger;

        public ItemService(IItemRepository itemRepository, IUserRepository userRepository, ILogger<ItemService> logger)
        {
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Item> Create(AddItemRequest item, string username, IFormFile? postedFile, CancellationToken cancel)
        {
            if (item is null)
            {
                const string message = "Invalid data to create item";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var user = await _userRepository.GetByUsername(username, cancel);

            if (user is null)
            {
                _logger.LogWarning("User {username} not found", username);
                throw new InvalidOperationException($"User {username} not found");
            }

            if (postedFile is null)
            {
                var createdItem = await _itemRepository.Create(item, user, cancel: cancel);

                _logger.LogInformation("Item {itemId} created", createdItem.ItemId);

                return createdItem;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var fileExtension = Path.GetExtension(postedFile.FileName);

            if (!allowedExtensions.Contains(fileExtension))
            {
                const string message = "Invalid file type. Only .jpg, .jpeg, and .png files are allowed";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var fileName = Guid.NewGuid() + fileExtension;

            var createdItemWithImage = await _itemRepository.Create(item, user, postedFile, fileName, cancel);

            _logger.LogInformation("Item {itemId} created", createdItemWithImage.ItemId);

            return createdItemWithImage;
        }

        public async Task<IEnumerable<Item>> GetAll(CancellationToken cancel = default)
        {
            var items = await _itemRepository.ListAll(cancel);

            _logger.LogInformation("All items retrieved");

            return items;
        }


        public async Task<Item?> GetById(long id, CancellationToken cancel)
        {
            var item = await _itemRepository.GetById(id, cancel);

            if (item is null)
            {
                _logger.LogWarning("Item {itemId} not found", id);
                throw new InvalidOperationException($"Item {id} not found");
            }

            _logger.LogInformation("Item {itemId} retrieved", item.ItemId);

            return item;
        }


        public async Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel)
        {
            var userItems = await _itemRepository.GetItemsOfUserBasedOnStatus(userId, active, cancel);

            if (userItems.Any())
            {
                return userItems;
            }

            var status = active ? "active" : "inactive";

            _logger.LogWarning("No {status} items found for user {userId}", status, userId);

            throw new InvalidOperationException($"No {status} items found for user {userId}");
        }

        public async Task<Item> Activate(long id, PublishItemRequest input, CancellationToken cancel = default)
        {
            const string format = "yyyy-MM-dd HH:mm";

            var dtInput = input.Expiration.ToString(CultureInfo.InvariantCulture);

            if (!DateTime.TryParseExact(dtInput, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExpiration))
            {
                const string message = "DateTime format is not correct. Correct format is: \"yyyy-MM-dd HH:mm\" ";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var now = DateTime.Now.ToString(format);

            DateTime.TryParse(now, out var dtNow);

            if (dtExpiration <= dtNow)
            {
                const string message = "Ends datetime cannot be set to a datetime earlier than the current datetime";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var item = await _itemRepository.Activate(id, input.Expiration, cancel);

            _logger.LogInformation("Activated item {itemId}", item.ItemId);

            return item;
        }

        public async Task Delete(long id, CancellationToken cancel = default)
        {
            var item = await _itemRepository.GetById(id, cancel);

            if (item is null)
            {
                _logger.LogWarning("Item {itemId} not found", id);
                throw new InvalidOperationException($"item {id} not found");
            }

            await _itemRepository.Delete(item, cancel);

            _logger.LogInformation("Deleted item {itemId}", id);
        }
    }
}
