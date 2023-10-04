using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Services
{
    public interface IUserService
    {
        public Task<User?> GetById(int id, CancellationToken cancel = default);

        public Task<User?> GetByUsername(string username, CancellationToken cancel = default);

        public Task<List<User>> GetAll(CancellationToken cancel = default);

        public Task<List<string>> GetAllUsernames(CancellationToken cancel = default);

        public Task<User> Create(UserCredentialsRequest input, CancellationToken cancel = default);
    }
}
