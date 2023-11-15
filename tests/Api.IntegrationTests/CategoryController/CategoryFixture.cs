using contracts.Requests.Category;
using contracts.Requests.User;
using Microsoft.Extensions.DependencyInjection;
using webapi.Database;
using webapi.Repository;

namespace Api.IntegrationTests.CategoryController
{
    public class CategoryFixture
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly UserRepository _userRepository;
        public HttpClient HttpClient { get; }
        public string AdminJwt { get; private set; }
        public string SimpleMainUserJwt { get; private set; } = string.Empty;


        public CategoryFixture()
        {
            var api = new ApiFactory();
            HttpClient = api.CreateClient();

            var context = api.Services.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();
            _categoryRepository = new CategoryRepository(context);
            _userRepository = new UserRepository(context);

            AdminJwt = Utils.LoginAdmin(HttpClient).GetAwaiter().GetResult();
            SeedCategories().GetAwaiter().GetResult();
            SeedSimpleUser().GetAwaiter().GetResult();
        }

        private async Task SeedCategories()
        {
            var simpleCategory1 = new AddCategoryRequest
            {
                Name = "Category 1"
            };

            var simpleCategory2 = new AddCategoryRequest
            {
                Name = "Category 2"
            };

            await _categoryRepository.Create(simpleCategory1);
            await _categoryRepository.Create(simpleCategory2);
        }

        private async Task SeedSimpleUser()
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
        }
    }
}
