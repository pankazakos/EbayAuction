using webapi.Contracts.Requests.Category;
using webapi.Models;

namespace webapi.Services
{
    public interface ICategoryService
    {
        public Task<Category?> GetById(int id, CancellationToken cancel = default);

        public Task<IEnumerable<Category>> GetAll(CancellationToken cancel = default);

        public Task<IEnumerable<Category>> FilterWithIds(List<int> ids, CancellationToken cancel = default);

        public Task<Category> Create(AddCategoryRequest input, CancellationToken cancel = default);

        public Task Delete(int id, CancellationToken cancel = default);

    }
}
