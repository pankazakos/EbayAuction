using contracts.Requests.Category;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Models;

namespace webapi.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IAuctionContext _dbContext;

        public CategoryRepository(IAuctionContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category?> GetById(int id, CancellationToken cancel = default)
        {
            return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Id == id, cancel);
        }

        public async Task<IEnumerable<Category>> GetAll(CancellationToken cancel = default)
        {
            return await _dbContext.Categories.ToListAsync(cancel);
        }

        public async Task<IEnumerable<Category>> FilterWithIds(List<int> ids, CancellationToken cancel = default)
        {
            var filteredCategories = _dbContext.Categories
                .Where(category => ids.Contains(category.Id));
                

            var filteredIds = filteredCategories.Select(category => category.Id);

            var sortedFilteredIds = await filteredIds.OrderBy(id => id).ToListAsync(cancel);
            var sortedInputIds = ids.OrderBy(id => id).ToList();

            if (!sortedFilteredIds.SequenceEqual(sortedInputIds))
            {
                throw new ArgumentException("Invalid category Ids");
            }

            return filteredCategories;
        }

        public async Task<Category> Create(AddCategoryRequest input, CancellationToken cancel = default)
        {
            var newCategory = new Category { Name = input.Name };

            _dbContext.Categories.Add(newCategory);

            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch(Exception)
            {
                _dbContext.Categories.Remove(newCategory);
            }

            return newCategory;
        }

        public async Task Delete(Category category, CancellationToken cancel = default)
        {
            _dbContext.Categories.Remove(category);

            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Categories.Add(category);
                throw;
            }
        }
    }
}
