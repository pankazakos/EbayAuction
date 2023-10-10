using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Repository
{
    public interface IUserRepository
    {
        public Task<User?> GetById(int id, CancellationToken cancel = default);

        public Task<User?> GetByUsername(string username, CancellationToken cancel = default);

        public Task<List<User>> GetAll(CancellationToken cancel = default);

        public Task<List<string>> GetAllUsernames(CancellationToken cancel = default);

        public Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default);

        public Task Delete(User user, CancellationToken cancel = default);

        public Task UpdateLastLogin(User user, CancellationToken cancel = default);
    }
}
