
using contracts.Requests.Category;
using contracts.Requests.User;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.ItemController
{
    public class ItemFixture
    {
        private readonly AuctionContext _context;
        public static HttpClient HttpClient { get; private set; } = new();

        public ItemFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();
            _context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            SeedCategories().GetAwaiter().GetResult();
            SeedDefaultSimpleUser().GetAwaiter().GetResult();
        }

        private async Task SeedCategories()
        {
            var categoryRepository = new CategoryRepository(_context);

            for (var i = 1; i <= 2; i++)
            {
                var input = new AddCategoryRequest
                {
                    Name = $"category {i}"
                };
                await categoryRepository.Create(input);
            }
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
        }
    }

}
