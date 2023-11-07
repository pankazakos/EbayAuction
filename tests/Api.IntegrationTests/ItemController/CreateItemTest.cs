using System.Net.Http.Headers;
using contracts.Requests.Item;
using Newtonsoft.Json;
using System.Text;
using contracts.Responses.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class CreateItemTest
    {
        private readonly HttpClient _client;
        private readonly string _simpleUserJwt;

        public CreateItemTest()
        {
            _client = new HttpClient();
            _simpleUserJwt = Utils.LoginOrCreateSimpleUser(_client).GetAwaiter().GetResult();
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

            await _client.DeleteAsync($"{Utils.BaseUrl}item/{createdItem.ItemId}");
        }


    }
}
