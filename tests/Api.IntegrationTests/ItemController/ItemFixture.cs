
using contracts.Requests.Category;
using contracts.Requests.Item;
using contracts.Requests.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapi;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.ItemController
{
    public class ItemFixture
    {
        private readonly AuctionContext _context;
        public static HttpClient HttpClient { get; private set; } = new();
        public static string SimpleMainUserJwt { get; private set; } = string.Empty;
        public static string SecondSimpleUserJwt { get; private set; } = string.Empty;
        public static string AdminJwt { get; private set; } = string.Empty;


        public ItemFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();
            _context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            AdminJwt = Utils.LoginAdmin(HttpClient).GetAwaiter().GetResult();
            SeedCategories().GetAwaiter().GetResult();
            SeedSimpleUsers().GetAwaiter().GetResult();

            var configuration = api.Services.GetRequiredService<IConfiguration>();
            SeedItemsOfSimpleUser(configuration).GetAwaiter().GetResult();
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

        private async Task SeedSimpleUsers()
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

            var loginResponse = await Utils.LoginUser(HttpClient,
                new LoginUserRequest { Username = "TestUser", Password = "password" });

            SimpleMainUserJwt = loginResponse.AccessToken;

            var secondUser = new RegisterUserRequest
            {
                Username = "second TestUser",
                Password = "password",
                Email = "secondTestUser@email.com",
                FirstName = "firstname",
                LastName = "lastname",
                Country = "testCountry",
                Location = "testLocation",
            };

            await userRepository.Create(secondUser);

            loginResponse = await Utils.LoginUser(HttpClient,
                new LoginUserRequest { Username = "second TestUser", Password = "password" });

            SecondSimpleUserJwt = loginResponse.AccessToken;
        }

        private async Task SeedItemsOfSimpleUser(IConfiguration configuration)
        {
            var categoryRepository = new CategoryRepository(_context);
            var userRepository = new UserRepository(_context);
            var itemRepository = new ItemRepository(_context, categoryRepository, configuration);

            var itemData = new AddItemRequest
            {
                Name = "default test item",
                CategoryIds = new List<int> { 1, 2 },
                FirstBid = 20,
                Description = "description of default test item"
            };

            var itemData2 = new AddItemRequest
            {
                Name = "default test item",
                CategoryIds = new List<int> { 1, 2 },
                FirstBid = 20,
                Description = "description of default test item"
            };

            var seller = await userRepository.GetByUsername("TestUser");

            if (seller is null)
            {
                throw new InvalidOperationException($"Cannot find user TestUser");
            }

            await itemRepository.Create(itemData, seller);

            await itemRepository.Create(itemData2, seller);
        }
    }
}
