using System.Net.Http.Headers;
using contracts.Requests.Item;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Models;
using webapi.Repository.Interfaces;

namespace webapi.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly IAuctionContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;

        public ItemRepository(IAuctionContext context, ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _dbContext = context;
            _categoryRepository = categoryRepository;
            _configuration = configuration;
        }

        public async Task<Item> Create(AddItemRequest item, User seller, IFormFile? postedFile = null, string? fileName = null, CancellationToken cancel = default)
        {
            var newItem = new Item
            {
                Name = item.Name,
                Currently = item.FirstBid,
                BuyPrice = item.BuyPrice,
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
                    var categories = await _categoryRepository.FilterWithIds(item.CategoryIds, cancel);

                    newItem.Categories = categories.ToList();

                    var directoryPath = _configuration.GetValue<string>("FileStorage:BasePath");

                    if (directoryPath is null)
                    {
                        throw new InvalidOperationException("Cannot find FileStorage:BasePath value in configuration");
                    }

                    Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

                    var fullPathToFile = directoryPath + fileName;

                    if (postedFile != null && fileName != null)
                    {
                        await using (var stream = new FileStream(fullPathToFile, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream, cancel);
                        }

                        newItem.ImageGuid = Path.GetFileNameWithoutExtension(fullPathToFile);
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

        public async Task<(IEnumerable<Item>, int)> Search(ItemSearchQuery query, CancellationToken cancel = default)
        {
            var baseQuery = _dbContext.Items.AsQueryable();

            // Get only published
            baseQuery = baseQuery.Where(item => item.Active && item.Ends != null && item.Ends > DateTime.Now);

            // Apply filters based on the query parameters
            if (!string.IsNullOrEmpty(query.Title))
            {
                baseQuery = baseQuery.Where(item => item.Name.Contains(query.Title));
            }

            if (query.Categories.Any())
            {
                baseQuery = baseQuery.Where(item => item.Categories.Any(category => query.Categories.Contains(category.Name)));
            }


            if (query.MinPrice > 0)
            {
                baseQuery = baseQuery.Where(item => item.Currently >= query.MinPrice);
            }

            if (query.MaxPrice > 0)
            {
                baseQuery = baseQuery.Where(item => item.Currently <= query.MaxPrice);
            }

            var totalCount = await baseQuery.CountAsync(cancel);

            // Apply pagination
            var items = await baseQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
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

        public async Task<List<Item>> GetBidden(long userId, CancellationToken cancel = default)
        {
            var items = await _dbContext.Items.Where(i => i.Active && i.NumBids > 0 && i.SellerId == userId)
                .ToListAsync(cancel);

            return items;
        }

        private string GetImageFilenameFullPath(string guid)
        {
            var directoryPath = _configuration.GetValue<string>("FileStorage:BasePath");

            if (directoryPath is null)
            {
                throw new InvalidOperationException("Cannot find FileStorage:BasePath value in configuration");
            }

            var extensions = new[] { ".jpg", ".jpeg", ".png" };

            string? fullPathToFile = null;

            foreach (var ext in extensions)
            {
                var testPath = Path.Combine(directoryPath, guid + ext);
                if (File.Exists(testPath))
                {
                    fullPathToFile = testPath;
                    break;
                }
            }

            if (fullPathToFile is null)
            {
                throw new InvalidOperationException("File does not exist");
            }

            return fullPathToFile;
        }

        public async Task<ByteArrayContent> GetImage(string guid, CancellationToken cancel = default)
        {
            var directoryPath = _configuration.GetValue<string>("FileStorage:BasePath");

            if (directoryPath is null)
            {
                throw new InvalidOperationException("Cannot find FileStorage:BasePath value in configuration");
            }

            var extensions = new[] { ".jpg", ".jpeg", ".png" };

            string? fullPathToFile = null;

            foreach (var ext in extensions)
            {
                var testPath = Path.Combine(directoryPath, guid + ext);
                if (File.Exists(testPath))
                {
                    fullPathToFile = testPath;
                    break;
                }
            }

            if (fullPathToFile is null)
            {
                throw new InvalidOperationException("File does not exist");
            }

            var imageBytes = await File.ReadAllBytesAsync(fullPathToFile, cancel);

            var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return content;
        }

        public async Task<List<Category>> GetCategories(long id, CancellationToken cancel = default)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.ItemId == id, cancel);

            if (item is null)
            {
                throw new InvalidOperationException($"Cannot find item {id}");
            }

            var categories = await _dbContext.Items
                .Include(i => i.Categories)
                .Where(i => i.ItemId == id)
                .SelectMany(i => i.Categories)
                .ToListAsync(cancel);

            return categories ?? throw new InvalidOperationException($"Cannot find categories of item {id}");
        }

        public async Task<Item> Activate(long id, DateTime expiration, CancellationToken cancel = default)
        {
            var item = await GetById(id, cancel);

            if (item is null)
            {
                throw new ArgumentException("Cannot find item with given id.");
            }

            if (item.Active)
            {
                throw new InvalidOperationException("Cannot activate an already published item");
            }

            item.Started = DateTime.Now;
            item.Ends = expiration;
            item.Active = true;

            await _dbContext.SaveChangesAsync(cancel);

            return item;
        }

        public async Task<Item> Edit(long id, EditItemRequest itemData, IFormFile? postedFile = null, string? fileName = null, CancellationToken cancel = default)
        {
            var item = await GetById(id, cancel);

            var itemWithCategories = _dbContext.Items
                .Include(i => i.Categories)
                .FirstOrDefault(i => i.ItemId == id);

            if (itemWithCategories is null)
            {
                throw new InvalidOperationException($"Cannot find item {id}");
            }

            if (item is null)
            {
                throw new ArgumentException($"Cannot find item {id}");
            }

            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancel))
            {
                try
                {
                    if (itemData.Name != null)
                    {
                        item.Name = itemData.Name;
                    }

                    if (itemData.CategoryIds != null)
                    {
                        var categoriesToRemove = itemWithCategories.Categories.ToList();

                        foreach (var oldCategory in categoriesToRemove)
                        {
                            itemWithCategories.Categories.Remove(oldCategory);
                        }


                        var newCategories = await _categoryRepository.FilterWithIds(itemData.CategoryIds, cancel);

                        foreach (var newCategory in newCategories)
                        {
                            item.Categories.Add(newCategory);
                        }
                    }

                    item.BuyPrice = itemData.BuyPrice;

                    if (itemData.FirstBid.HasValue)
                    {
                        item.FirstBid = itemData.FirstBid.Value;
                        item.Currently = itemData.FirstBid.Value;
                    }

                    if (itemData.Description != null)
                    {
                        item.Description = itemData.Description;
                    }

                    var directoryPath = _configuration.GetValue<string>("FileStorage:BasePath");

                    if (directoryPath is null)
                    {
                        throw new InvalidOperationException("Cannot find FileStorage:BasePath value in configuration");
                    }

                    Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

                    var fullPathToNewImage = directoryPath + fileName;

                    if (postedFile != null)
                    {
                        // Delete previous image if it exists
                        if (item.ImageGuid != null)
                        {

                            var previousImageFullFilename = GetImageFilenameFullPath(item.ImageGuid);
                            File.Delete(previousImageFullFilename);
                        }

                        // Save new image
                        await using (var stream = new FileStream(fullPathToNewImage, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream, cancel);
                        }

                        item.ImageGuid = Path.GetFileNameWithoutExtension(fullPathToNewImage);
                    }

                    await _dbContext.SaveChangesAsync(cancel);

                    await transaction.CommitAsync(cancel);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancel);
                    throw;
                }
            }
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
