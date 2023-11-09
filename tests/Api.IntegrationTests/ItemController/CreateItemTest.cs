using System.Net.Http.Headers;
using contracts.Requests.Item;
using Newtonsoft.Json;
using System.Text;
using contracts.Requests.User;
using contracts.Responses.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class CreateItemTest : IClassFixture<CreateItemFixture>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;
        private readonly string _simpleUserJwt;

        public CreateItemTest()
        {
            _factory = new ApiFactory();
            _client = _factory.CreateClient();
            var userCredentials = new LoginUserRequest
            {
                Username = "admin",
                Password = "admin"
            };
            _simpleUserJwt = Utils.LoginUser(_client, userCredentials).GetAwaiter().GetResult().AccessToken;
        }

        [Fact]
        public async Task CreateItem_ReturnsItem_WhenInputIsCorrectWithoutImage()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _simpleUserJwt);

            var itemData = new AddItemRequest
            {
                Name = "test item",
                FirstBid = 50,
                CategoryIds = new List<int> { 1, 2 },
                Description = "test item description"
            };

            var addItemBody = new StringContent(JsonConvert.SerializeObject(itemData), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{Utils.BaseUrl}item", addItemBody);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var createdItem = JsonConvert.DeserializeObject<AddItemResponse>(responseString);

            createdItem.Should().NotBeNull();

            createdItem!.Name.Should().Be(itemData.Name);

            var adminJwt = await Utils.LoginAdmin(_client);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);

            await _client.DeleteAsync($"{Utils.BaseUrl}item/{createdItem.ItemId}");
        }


    }
}
