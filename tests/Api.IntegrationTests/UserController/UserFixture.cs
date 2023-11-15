using contracts.Requests.User;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.UserController
{
    public class UserFixture
    {
        private readonly AuctionContext _context;
        public HttpClient HttpClient { get; }
        public string AdminJwt { get; private set; } 
        public LoginUserRequest SimpleUserCredentials { get; private set; } = new();

        public UserFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();
            _context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            AdminJwt = Utils.LoginAdmin(HttpClient).GetAwaiter().GetResult();
            SeedDefaultSimpleUser().GetAwaiter().GetResult();
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
    }
}
