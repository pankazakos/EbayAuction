using contracts.Requests.User;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.UserController
{
    public class UserFixture
    {
        private readonly AuctionContext _context;
        public static HttpClient HttpClient { get; private set; } = new();
        public static string AdminJwt { get; private set; } = string.Empty;
        public static LoginUserRequest SimpleUserCredentials { get; private set; } = new();

        public static int IdUserToRemove { get; private set; }

        public UserFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();
            _context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            LoginAdmin().GetAwaiter().GetResult();
            SeedDefaultSimpleUser().GetAwaiter().GetResult();
            SeedUserToRemove().GetAwaiter().GetResult();
        }

        private static async Task LoginAdmin()
        {
            var jwt = await Utils.LoginAdmin(HttpClient);

            AdminJwt = jwt;
        }

        private async Task SeedDefaultSimpleUser()
        {
            var userRepository = new UserRepository(_context);

            var userInfo = new RegisterUserRequest
            {
                Username = "TestUser",
                Password = "password",
                Email = "testUser@email.com",
                FirstName = "firstname",
                LastName = "lastname",
                Country = "testCountry",
                Location = "testLocation",
            };

            await userRepository.Create(userInfo);

            SimpleUserCredentials = new LoginUserRequest { Username = "TestUser", Password = "password" };
        }

        private async Task SeedUserToRemove()
        {
            var userRepository = new UserRepository(_context);

            var userInfo = new RegisterUserRequest
            {
                Username = "TestUserToRemove",
                Password = "password",
                Email = "testUserToRemove@email.com",
                FirstName = "firstname",
                LastName = "lastname",
                Country = "testCountry",
                Location = "testLocation",
            };

            var user = await userRepository.Create(userInfo);

            IdUserToRemove = user.Id;
        }
    }
}
