using contracts.Requests.Bid;
using contracts.Requests.Category;
using contracts.Requests.Item;
using contracts.Requests.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.BidController
{
    public class BidFixture
    {
        private readonly BidRepository _bidRepository;
        private readonly UserRepository _userRepository;
        private readonly ItemRepository _itemRepository;
        private readonly CategoryRepository _categoryRepository;
        public HttpClient HttpClient { get; }
        public string MainUserJwt { get; private set; } = string.Empty;
        public string SecondaryUserJwt { get; private set; } = string.Empty;

        public BidFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();

            var context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();

            _bidRepository = new BidRepository(context);
            _userRepository = new UserRepository(context);

            _categoryRepository = new CategoryRepository(context);
            var configuration = api.Services.GetRequiredService<IConfiguration>();

            _itemRepository = new ItemRepository(context, _categoryRepository, configuration);

            SeedUsers().GetAwaiter().GetResult();
            SeedBids().GetAwaiter().GetResult();
        }

        private async Task SeedUsers()
        {
            var mainUser = new RegisterUserRequest
            {
                Username = "MainUser",
                Password = "password",
                Email = "MainUser@test.com",
                FirstName = "firstname",
                LastName = "lastname",
                Country = "test country",
                Location = "test location"
            };

            var secondaryUser = new RegisterUserRequest
            {
                Username = "SecondaryUser",
                Password = "password",
                Email = "SecondaryUser@test.com",
                FirstName = "firstname",
                LastName = "lastname",
                Country = "test country",
                Location = "test location"
            };

            await _userRepository.Create(mainUser);
            await _userRepository.Create(secondaryUser);

            MainUserJwt = (await Utils.LoginUser(HttpClient, new LoginUserRequest { Username = "MainUser", Password = "password" })).AccessToken;
            SecondaryUserJwt = (await Utils.LoginUser(HttpClient, new LoginUserRequest { Username = "SecondaryUser", Password = "password" })).AccessToken;
        }

        private async Task SeedBids()
        {
            var categoryRequest = new AddCategoryRequest
            {
                Name = "Category 1"
            };

            await _categoryRepository.Create(categoryRequest);

            var itemRequest = new AddItemRequest
            {
                Name = "first test item",
                CategoryIds = new List<int> { 1 },
                FirstBid = 20,
                Description = "description of first test item"
            };

            var secondItemRequest = new AddItemRequest
            {
                Name = "second test item",
                CategoryIds = new List<int> { 1 },
                FirstBid = 10,
                Description = "description of second test item"
            };

            var seller = await _userRepository.GetByUsername("MainUser")
                         ?? throw new InvalidOperationException($"Cannot find user MainUser");

            await _itemRepository.Create(itemRequest, seller);

            await _itemRepository.Create(secondItemRequest, seller);

            var firstBid = new AddBidRequest
            {
                ItemId = 1,
                Amount = 20
            };

            var secondBid = new AddBidRequest
            {
                ItemId = firstBid.ItemId,
                Amount = 30
            };

            var secondaryUser = await _userRepository.GetByUsername("SecondaryUser");

            var firstItem = await _itemRepository.GetById(firstBid.ItemId);

            if (secondaryUser is null || firstItem is null)
            {
                throw new InvalidOperationException("Cannot get mainUser or firstItem");
            }

            await _bidRepository.Create(firstBid, secondaryUser, firstItem);

            await _bidRepository.Create(secondBid, secondaryUser, firstItem);
        }
    }
}
