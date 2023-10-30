using webapi.Contracts.Requests.User;
using webapi.Models;
using webapi.Repository;

namespace webapi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetById(int id, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetById(id, cancel);

            if (user == null)
            {
                _logger.LogInformation("User GetById: No user found");
                return null;
            }

            _logger.LogInformation("User GetById: Retrieved user with id: {id}", id);

            return user;
        }

        public async Task<User?> GetByUsername(string username, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetByUsername(username, cancel);

            if (user == null)
            {
                _logger.LogInformation("User GetByUsername: No user found");
                return null;
            }

            _logger.LogInformation("User GetByUsername: Retrieved user with username: {username}", username);

            return user;
        }

        public async Task<IEnumerable<User>> GetAll(CancellationToken cancel = default)
        {
            var users = await _userRepository.GetAll(cancel);

            _logger.LogInformation(!users.Any() ? "User GetAll: No users found" : "User GetAll [\u001b[31msuccess\u001b[0m]: All users retrieved");

            return users;
        }

        public async Task<List<string>> GetAllUsernames(CancellationToken cancel = default)
        {
            var usernames = await _userRepository.GetAllUsernames(cancel);

            if (!usernames.Any())
            {
                _logger.LogInformation("User GetAllUsernames: No users found");
            }
            else
            {
                _logger.LogInformation("User GetAllUsernames: Retrieved all usernames");
            }

            return usernames;
        }

        public async Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default)
        {
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
            {
                _logger.LogInformation("User Create: Client did not specify username or password");
                throw new ArgumentException("Username and password are required.");
            }

            var user = await _userRepository.GetByUsername(input.Username, cancel);

            if (user != null)
            {
                _logger.LogInformation("User Create: create failed. Username already exists");
                throw new ArgumentException("Username already exists.");
            }

            var createdUser = await _userRepository.Create(input, cancel);

            _logger.LogInformation("User Create: User successfully created");

            return createdUser;
        }

        public async Task Delete(int id, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetById(id, cancel);

            if (user is null)
            {
                _logger.LogInformation("User Delete failed: no user found");
                throw new InvalidOperationException("User not Found");
            }

            await _userRepository.Delete(user, cancel);

            _logger.LogInformation("User Delete: Success");
        }

        public async Task UpdateLastLogin(string username, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetByUsername(username, cancel);

            if (user is null)
            {
                _logger.LogInformation("User UpdateLastLogin failed: no user found");
                throw new InvalidOperationException("User not found");
            }

            await _userRepository.UpdateLastLogin(user, cancel);

            _logger.LogInformation("User UpdateLastLogin: Success");
        }

    }
}
