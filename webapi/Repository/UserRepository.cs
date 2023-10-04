using Microsoft.EntityFrameworkCore;
using webapi.Contracts.Requests;
using webapi.Database;
using webapi.Models;
using webapi.Utilities;

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

        public async Task<List<User>> GetAll(CancellationToken cancel = default)
        {
            var users = await _dbContext.Users.ToListAsync(cancel);
            return users;
        }

        public async Task<List<string>> GetAllUsernames(CancellationToken cancel = default)
        {
            var usernames = await _dbContext.Users.Select(u => u.Username).ToListAsync(cancel);
            return usernames;
        }

        public async Task<User> Create(UserCredentialsRequest input, CancellationToken cancel = default)
        {
            var salt = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPassword(input.Password, salt);

            // Create a new user
            var user = new User
            {
                Username = input.Username,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                FirstName = "First_Name",
                LastName = "Last_Name",
                LastLogin = null,
                DateJoined = DateTime.Now,
                Email = $"{input.Username}@email.com",
                Country = "USA",
                Location = "Chicago",
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
    }
}
