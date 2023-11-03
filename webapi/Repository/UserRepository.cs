using contracts.Requests.User;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Models;
using webapi.Utilities.AuthorizationUtils.PasswordUtils;

namespace webapi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IAuctionContext _dbContext;

        public UserRepository(IAuctionContext context)
        {
            _dbContext = context;
        }

        public async Task<User?> GetById(int id, CancellationToken cancel = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id, cancel);
            return user;
        }

        public async Task<User?> GetByUsername(string username, CancellationToken cancel = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username, cancel);
            return user;
        }

        public async Task<(IEnumerable<User>, int)> GetAllPaged(int page, int limit, CancellationToken cancel = default)
        {
            var totalCount = await _dbContext.Users.CountAsync(cancel);
            var users = await _dbContext.Users
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancel);

            return (users, totalCount);
        }

        public async Task<List<string>> GetAllUsernames(CancellationToken cancel = default)
        {
            var usernames = await _dbContext.Users.Select(u => u.Username).ToListAsync(cancel);
            return usernames;
        }

        public async Task<User> Create(RegisterUserRequest input, CancellationToken cancel = default)
        {
            var salt = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPassword(input.Password, salt);

            var user = new User
            {
                Username = input.Username,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                FirstName = input.FirstName,
                LastName = input.LastName,
                LastLogin = null,
                DateJoined = DateTime.Now,
                Email = $"{input.Username}@email.com",
                Country = input.Country,
                Location = input.Location,
                IsSuperuser = false,
                IsActive = true
            };

            _dbContext.Users.Add(user);

            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Users.Remove(user);
                throw;
            }

            return user;
        }

        public async Task Delete(User user, CancellationToken cancel = default)
        {
            _dbContext.Users.Remove(user);
            try
            {
                await _dbContext.SaveChangesAsync(cancel);
            }
            catch (Exception)
            {
                _dbContext.Users.Add(user);
                throw;
            }
        }

        public async Task UpdateLastLogin(User user, CancellationToken cancel = default)
        {
            user.LastLogin = DateTime.Now;

            await _dbContext.SaveChangesAsync(cancel);
        }

        public async Task<int> UsernameToId(string username, CancellationToken cancel = default)
        {
            var user = await GetByUsername(username, cancel);

            if (user is null)
            {
                throw new InvalidOperationException("Username not found");
            }

            return user.Id;
        }
    }
}
