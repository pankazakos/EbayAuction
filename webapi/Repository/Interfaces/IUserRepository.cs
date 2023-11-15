using contracts.Requests.User;
using webapi.Models;

namespace webapi.Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetById(int id, CancellationToken cancel = default);

        public Task<User?> GetByUsername(string username, CancellationToken cancel = default);

        public Task<(IEnumerable<User>, int)> GetAllPaged(int page, int pageSize, CancellationToken cancel = default);

        public Task<List<string>> GetAllUsernames(CancellationToken cancel = default);

        public Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default);

        public Task Delete(User user, CancellationToken cancel = default);

        public Task UpdateLastLogin(User user, CancellationToken cancel = default);

        public Task<int> UsernameToId(string username, CancellationToken cancel = default);
    }
}
