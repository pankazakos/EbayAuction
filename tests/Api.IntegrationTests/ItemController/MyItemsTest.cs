using System.Net;
using System.Net.Http.Headers;
using contracts.Responses.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class MyItemsTest : IClassFixture<ItemFixture>
    {
        private readonly ItemFixture _fixture;
        private readonly HttpClient _client;
        public MyItemsTest(ItemFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.SimpleMainUserJwt);
        }


        [Fact]
        public async Task Inactive_ReturnsInactiveItems_WhenItemsExist()
        {
            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/inactive");

            response.EnsureSuccessStatusCode();

            var items = await Utils.ConvertResponseData<IEnumerable<BasicItemResponse>>(response);

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
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.AdminJwt);

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/inactive");

            response.EnsureSuccessStatusCode();

            var items = await Utils.ConvertResponseData<IEnumerable<BasicItemResponse>>(response);

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
