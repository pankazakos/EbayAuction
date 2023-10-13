using webapi.Contracts.Requests.Category;
using webapi.Models;

namespace webapi.Repository
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetById(int id, CancellationToken cancel = default);

        public Task<IEnumerable<Category>> GetAll(CancellationToken cancel = default);

        public Task<IEnumerable<Category>> FilterWithIds(List<int> ids, CancellationToken cancel = default);

        public Task<Category> Create(AddCategoryRequest input, CancellationToken cancel = default);

        public Task Delete(Category category, CancellationToken cancel = default);
    }
}
