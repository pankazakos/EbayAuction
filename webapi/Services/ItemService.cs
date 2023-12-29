using contracts.Requests.Item;
using webapi.Models;
using webapi.Repository.Interfaces;
using webapi.Services.Interfaces;

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
            try
            {
                item.Validate();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
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

        public async Task<(IEnumerable<Item>, int)> Search(ItemSearchQuery query, CancellationToken cancel = default)
        {
            try
            {
                query.Validate();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }

            var response = await _itemRepository.Search(query, cancel);

            _logger.LogInformation("Search items: page={page}, limit={limit}, title={title}, categories={categories}, minPrice={minPrice}, maxPrice={maxPrice}", 
                query.Page, query.Limit, query.Title, query.Categories, query.MinPrice, query.MaxPrice);

            return (response.Item1, response.Item2);
        }


        public async Task<Item?> GetById(long id, CancellationToken cancel)
        {
            var item = await _itemRepository.GetById(id, cancel);

            if (item is null)
            {
                _logger.LogWarning("Item {itemId} not found", id);
                return null;
            }

            _logger.LogInformation("Item {itemId} retrieved", item.ItemId);

            return item;
        }


        public async Task<IEnumerable<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel)
        {
            var userItems = await _itemRepository.GetItemsOfUserBasedOnStatus(userId, active, cancel);

            if (userItems.Any())
            {
                return userItems;
            }

            var status = active ? "active" : "inactive";

            _logger.LogWarning("No {status} items found for user {userId}", status, userId);

            return userItems;
        }

        public async Task<IEnumerable<Item>> GetBidden(long userId, CancellationToken cancel = default)
        {
            var itemsWithBids = await _itemRepository.GetBidden(userId, cancel);

            if (itemsWithBids.Any())
            {
                return itemsWithBids;
            }

            _logger.LogWarning("No bidden items found for user {userId}", userId);

            return itemsWithBids;
        }

        public async Task<ByteArrayContent> GetImage(string guid, CancellationToken cancel = default)
        {
            var content = await _itemRepository.GetImage(guid, cancel);

            return content;
        }

        public async Task<IEnumerable<Category>> GetCategories(long id, CancellationToken cancel = default)
        {
            var categories = await _itemRepository.GetCategories(id, cancel);

            if (categories.Any())
            {
                _logger.LogInformation("Categories {categoryIds} retrieved", string.Join(", ", categories.Select(c => c.Id)));
                return categories;
            }

            _logger.LogWarning("No categories found for item {itemId}", id);

            return Enumerable.Empty<Category>();
        }

        public async Task<Item> Activate(long id, PublishItemRequest input, CancellationToken cancel = default)
        {
            try
            {
                input.Validate();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }

            try
            {
                var item = await _itemRepository.Activate(id, input.Expiration, cancel);

                _logger.LogInformation("Published item {itemId}", item.ItemId);
                return item;
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Item {itemId} is already published", id);
                throw;
            }
        }

        public async Task<Item> Edit(long id, EditItemRequest itemData, IFormFile? postedFile = null, CancellationToken cancel = default)
        {
            if (postedFile is null)
            {
                var editedItem = await _itemRepository.Edit(id, itemData, cancel: cancel);

                _logger.LogInformation("Item {itemId} edited", editedItem.ItemId);

                return editedItem;
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

            var editedItemWithImage = await _itemRepository.Edit(id, itemData, postedFile, fileName, cancel);

            _logger.LogInformation("Item {itemId} edited", editedItemWithImage.ItemId);

            return editedItemWithImage;
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
