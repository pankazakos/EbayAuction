using contracts.Requests.Item;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using webapi.Contracts.Requests.Item;
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

        public async Task<Item> Create(AddItemRequest item, User seller, IFormFile? postedFile, string? imageFilename, CancellationToken cancel = default)
        {
            var newItem = new Item
            {
                Name = item.Name,
                Currently = item.FirstBid,
                BuyPrice = null,
                FirstBid = item.FirstBid,
                NumBids = 0,
                Started = null,
                Ends = null,
                Active = false,
                Description = item.Description,
                SellerId = seller.Id,
                Seller = seller
            };

            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancel))
            {
                try
                {
                    var categories = await _categoryService.FilterWithIds(item.CategoryIds, cancel);

                    newItem.Categories = categories.ToList();

                    if (postedFile != null && imageFilename != null)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "item-images", imageFilename);

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream, cancel);
                        }

                        newItem.ImageGuid = Path.GetFileNameWithoutExtension(imageFilename);
                    }

                    _dbContext.Items.Add(newItem);

                    await _dbContext.SaveChangesAsync(cancel);

                    await transaction.CommitAsync(cancel);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancel);
                    throw;
                }
            }

            return newItem;
        }

        public async Task<(IEnumerable<Item>, int)> GetAllPaged(int page, int limit, CancellationToken cancel = default)
        {
            var totalCount = await _dbContext.Items.CountAsync(cancel);
            var items = await _dbContext.Items
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancel);

            return (items, totalCount);
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

        public async Task Delete(Item item, CancellationToken cancel = default)
        {
            _dbContext.Items.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Items.Add(item);
                throw;
            }
        }
    }
}
