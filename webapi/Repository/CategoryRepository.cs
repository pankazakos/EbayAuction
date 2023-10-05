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
            return await _dbContext.Categories
                .Where(category => ids.Contains(category.Id))
                .ToListAsync(cancel);
        }

        public async Task<Category> Create(string name, CancellationToken cancel = default)
        {
            var newCategory = new Category { Name = name };

            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync(cancel);

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
