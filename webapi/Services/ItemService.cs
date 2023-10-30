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

        public ItemService(IItemRepository itemRepository, IUserRepository userRepository)
        {
            _itemRepository = itemRepository;
            _userRepository = userRepository;
        }

        public async Task<Item> Create(AddItemRequest item, string username, IFormFile? postedFile, CancellationToken cancel)
        {
            if (item is null)
            {
                throw new ArgumentException("Invalid data.");
            }

            var user = await _userRepository.GetByUsername(username, cancel);

            if (user is null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (postedFile is null)
            {
                return await _itemRepository.Create(item, user, cancel: cancel);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var fileExtension = Path.GetExtension(postedFile.FileName);

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, and .png files are allowed.");
            }

            var fileName = Guid.NewGuid() + fileExtension;

            return await _itemRepository.Create(item, user, postedFile, fileName, cancel);
        }

        public async Task<IEnumerable<Item>> ListAll(CancellationToken cancel = default)
        {
            return await _itemRepository.ListAll(cancel);
        }


        public async Task<Item?> GetById(long id, CancellationToken cancel)
        {
            return await _itemRepository.GetById(id, cancel);
        }


        public async Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel)
        {
            return await _itemRepository.GetItemsOfUserBasedOnStatus(userId, active, cancel);
        }

        public async Task<Item> Activate(long id, PublishItemRequest input, CancellationToken cancel = default)
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

        public async Task Delete(long id, CancellationToken cancel = default)
        {
            var item = await _itemRepository.GetById(id, cancel);

            if (item is null)
            {
                throw new InvalidOperationException("item not found");
            }

            await _itemRepository.Delete(item, cancel);
        }
    }
}
