using webapi.Database;
using webapi.Models;
using webapi.Repository;

namespace webapi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category?> GetById(int id, CancellationToken cancel = default)
        {
            return await _categoryRepository.GetById(id, cancel);
        }

        public async Task<IEnumerable<Category>> GetAll(CancellationToken cancel = default)
        {
            return await _categoryRepository.GetAll(cancel);
        }

        public async Task<IEnumerable<Category>> FilterWithIds(List<int> ids, CancellationToken cancel = default)
        {
            return await _categoryRepository.FilterWithIds(ids, cancel);
        }

        public async Task<Category> Create(string name, CancellationToken cancel = default)
        {
            return await _categoryRepository.Create(name, cancel);
        }

        public async Task Delete(int id, CancellationToken cancel = default)
        {
            var category = await _categoryRepository.GetById(id, cancel);

            if (category is not null)
            {
                await _categoryRepository.Delete(category, cancel);
            }
            else
            {
                throw new InvalidOperationException("Category not found");
            }
        }
    }
}
