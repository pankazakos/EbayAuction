using webapi.Contracts.Requests;
using webapi.Models;

namespace webapi.Services
{
    public interface IUserService
    {
        public Task<User?> GetById(int id, CancellationToken cancel);

        public Task<User?> GetByUsername(string username);

        public Task<List<User>> GetAll(CancellationToken cancel);

        public Task<List<string>> GetAllUsernames(CancellationToken cancel);

        public Task<User> Create(CreateUserRequest input, CancellationToken cancel);
    }
}
