using contracts.Requests.User;
using webapi.Models;

namespace webapi.Services
{
    public interface IUserService
    {
        public Task<User?> GetById(int id, CancellationToken cancel = default);

        public Task<User?> GetByUsername(string username, CancellationToken cancel = default);

        public Task<(IEnumerable<User>, int)> GetAllPaged(int page, int limit, CancellationToken cancel = default);

        public Task<List<string>> GetAllUsernames(CancellationToken cancel = default);

        public Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default);

        public Task Delete(int id, CancellationToken cancel = default);

        public Task UpdateLastLogin(string username, CancellationToken cancel = default);
    }
}
