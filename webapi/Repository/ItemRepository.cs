using Microsoft.EntityFrameworkCore;
using webapi.Contracts.Requests;
using webapi.Database;
using webapi.Models;
using webapi.Services;

namespace webapi.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly IAuctionContext _dbContext;
        private readonly ICategoryService _categoryService;

        public ItemRepository(IAuctionContext context, ICategoryService categoryService)
        {
            _dbContext = context;
            _categoryService = categoryService;
        }

        public async Task<Item> Create(CreateItemRequest item, CancellationToken cancel = default)
        {
            var newItem = new Item
            {
                Name = item.Name,
                FirstBid = item.FirstBid,
                Currently = item.FirstBid,
                Active = false,
                Started = item.Started,
                Ends = item.Ends,
                SellerId = item.SellerId,
                Description = item.Description,
            };

            var categories = await _categoryService.FilterWithIds(item.CategoryIds, cancel);

            newItem.Categories = new List<Category>(categories);

            _dbContext.Items.Add(newItem);

            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Items.Remove(newItem);
                throw;
            }

            return newItem;
        }

        public async Task<Item?> GetById(long id, CancellationToken cancel = default)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.ItemId == id, cancel);

            return item;
        }


        public async Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel = default)
        {
            var items = await _dbContext.Items.Where(i => i.Active == active && i.SellerId == userId).ToListAsync(cancel);

            return items;
        }

        public async Task<Item> Activate(long id, DateTime expiration, CancellationToken cancel = default)
        {
            var item = await GetById(id, cancel);

            if (item is null)
            {
                throw new ArgumentException("Cannot find item with given id.");
            }

            item.Started = DateTime.Now;
            item.Ends = expiration;
            item.Active = true;

            await _dbContext.SaveChangesAsync(cancel);

            return item;
        }
    }
}
