using System.Net;
using System.Net.Http.Headers;
using contracts.Responses.Item;
using FluentAssertions;
using Newtonsoft.Json;

namespace Api.IntegrationTests.ItemController
{
    [Collection("Item Collection")]
    public class MyItemsTest : IClassFixture<ItemFixture>
    {
        private readonly HttpClient _client;
        public MyItemsTest()
        {
            _client = ItemFixture.HttpClient;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ItemFixture.SimpleMainUserJwt);
        }


        [Fact]
        public async Task Inactive_ReturnsInactiveItems_WhenItemsExist()
        {
            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/inactive");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<IEnumerable<BasicItemResponse>>(responseString);

            // Assert
            if (items is null)
            {
                throw new InvalidOperationException("could not deserialize response");
            }

            items.Count().Should().NotBe(0);
        }


        [Fact]
        public async Task Inactive_ReturnsEmptyList_WhenItemsDoNotExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ItemFixture.AdminJwt);

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/inactive");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<IEnumerable<BasicItemResponse>>(responseString);

            // Assert
            if (items is null)
            {
                throw new InvalidOperationException("could not deserialize response");
            }

            items.Count().Should().Be(0);
        }


        [Fact]
        public async Task Inactive_ReturnsUnauthorized_WhenNoAuthorizationGiven()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/inactive");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
