using Microsoft.EntityFrameworkCore;
using webapi.Contracts.Requests;
using webapi.Database;
using webapi.Models;

namespace webapi.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly IAuctionContext _dbContext;

        public ItemRepository(IAuctionContext context)
        {
            _dbContext = context;
        }

        public async Task<Item> Create(CreateItemRequest item, CancellationToken cancel)
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

            var categories = await _dbContext.Categories.Where(c => item.CategoryIds.Contains(c.Id)).ToListAsync(cancel);

            newItem.Categories = categories;

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

        public async Task<Item?> GetById(long id, CancellationToken cancel)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.ItemId == id, cancel);

            return item;
        }


        public async Task<List<Item>> GetItemsOfUserBasedOnStatus(int userId, bool active, CancellationToken cancel)
        {
            var items = await _dbContext.Items.Where(i => i.Active == active && i.SellerId == userId).ToListAsync(cancel);

            return items;
        }
    }
}
