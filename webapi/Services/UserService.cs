using webapi.Contracts.Requests;
using webapi.Models;
using webapi.Repository;

namespace webapi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetById(int id, CancellationToken cancel)
        {
            return await _userRepository.GetById(id, cancel);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _userRepository.GetByUsername(username);
        }

        public async Task<List<User>> GetAll(CancellationToken cancel)
        {
            return await _userRepository.GetAll(cancel);
        }

        public async Task<List<string>> GetAllUsernames(CancellationToken cancel)
        {
            return await _userRepository.GetAllUsernames(cancel);
        }

        public async Task<User> Create(CreateUserRequest input, CancellationToken cancel)
        {
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            var user = await _userRepository.GetByUsername(input.Username);

            if (user != null)
            {
                throw new ArgumentException("Username already exists.");
            }

            return await _userRepository.Create(input, cancel);
        }
    }
}
