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
                _logger.LogWarning("User not found");
                return null;
            }

            _logger.LogInformation("Retrieved user with id: {id}", id);

            return user;
        }

        public async Task<User?> GetByUsername(string username, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetByUsername(username, cancel);

            if (user == null)
            {
                _logger.LogWarning("User not found");
                return null;
            }

            _logger.LogInformation("Retrieved user {username}", username);

            return user;
        }

        public async Task<IEnumerable<User>> GetAll(CancellationToken cancel = default)
        {
            var users = await _userRepository.GetAll(cancel);

            _logger.LogInformation("Retrieved all users");

            return users;
        }

        public async Task<List<string>> GetAllUsernames(CancellationToken cancel = default)
        {
            var usernames = await _userRepository.GetAllUsernames(cancel);

            if (!usernames.Any())
            {
                _logger.LogWarning("No users found");
            }
            else
            {
                _logger.LogInformation("Retrieved all usernames");
            }

            return usernames;
        }

        public async Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default)
        {
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
            {
                const string message = "Username and password are required";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var user = await _userRepository.GetByUsername(input.Username, cancel);

            if (user != null)
            {
                const string message = "Username already exists";
                _logger.LogWarning(message);
                throw new ArgumentException(message);
            }

            var createdUser = await _userRepository.Create(input, cancel);

            _logger.LogInformation("User {username} created", createdUser.Username);

            return createdUser;
        }

        public async Task Delete(int id, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetById(id, cancel);

            if (user is null)
            {
                const string message = "User not found";
                _logger.LogWarning(message);
                throw new InvalidOperationException(message);
            }

            await _userRepository.Delete(user, cancel);

            _logger.LogInformation("Deleted user {username}", user.Username);
        }

        public async Task UpdateLastLogin(string username, CancellationToken cancel = default)
        {
            var user = await _userRepository.GetByUsername(username, cancel);

            if (user is null)
            {
                const string message = "User not found";
                _logger.LogWarning(message);
                throw new InvalidOperationException(message);
            }

            await _userRepository.UpdateLastLogin(user, cancel);

            _logger.LogInformation("User {username} logged in", user.Username);
        }

    }
}
