using contracts.Requests.Bid;
using contracts.Requests.Category;
using contracts.Requests.Item;
using contracts.Requests.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.ItemController
{
    public class ItemFixture
    {
        private readonly ItemRepository _itemRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly UserRepository _userRepository;
        private readonly BidRepository _bidRepository;
        public HttpClient HttpClient { get; }
        public string SimpleMainUserJwt { get; private set; } = string.Empty;
        public string SecondSimpleUserJwt { get; private set; } = string.Empty;
        public string AdminJwt { get; private set; }


        public ItemFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();
            var context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            var configuration = api.Services.GetRequiredService<IConfiguration>();
            _userRepository = new UserRepository(context);
            _categoryRepository = new CategoryRepository(context);
            _itemRepository = new ItemRepository(context, _categoryRepository, configuration);
            _bidRepository = new BidRepository(context);

            AdminJwt = Utils.LoginAdmin(HttpClient).GetAwaiter().GetResult();
            SeedCategories().GetAwaiter().GetResult();
            SeedSimpleUsers().GetAwaiter().GetResult();


            SeedItemsOfSimpleUser().GetAwaiter().GetResult();
        }

        private async Task SeedCategories()
        {
            for (var i = 1; i <= 2; i++)
            {
                var input = new AddCategoryRequest
                {
                    Name = $"category {i}"
                };
                await _categoryRepository.Create(input);
            }
        }

        private async Task SeedSimpleUsers()
        {
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

            await _userRepository.Create(userInfo);

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

            await _userRepository.Create(secondUser);

            loginResponse = await Utils.LoginUser(HttpClient,
                new LoginUserRequest { Username = "second TestUser", Password = "password" });

            SecondSimpleUserJwt = loginResponse.AccessToken;
        }

        private async Task SeedItemsOfSimpleUser()
        {
            var firstItem = new AddItemRequest
            {
                Name = "first test item",
                CategoryIds = new List<int> { 1, 2 },
                FirstBid = 20,
                Description = "description of default test item"
            };

            var secondItem = new AddItemRequest
            {
                Name = "second test item",
                CategoryIds = new List<int> { 1, 2 },
                FirstBid = 20,
                Description = "description of default test item"
            };

            var seller = await _userRepository.GetByUsername("TestUser") 
                         ?? throw new InvalidOperationException("Cannot find user TestUser");

            var bidder = await _userRepository.GetByUsername("second TestUser") ??
                         throw new InvalidOperationException("Cannot find user second TestUser");

            await _itemRepository.Create(firstItem, seller);
            await _itemRepository.Create(secondItem, seller);

            var bid = new AddBidRequest
            {
                ItemId = 2,
                Amount = 30.5m
            };

            var item2 = await _itemRepository.GetById(2) ?? throw new InvalidOperationException("Cannot find item 2");

            await _bidRepository.Create(bid, bidder, item2);
        }
    }
}
