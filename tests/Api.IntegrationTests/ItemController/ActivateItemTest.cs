using System.Net;
using System.Net.Http.Headers;
using contracts.Requests.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class ActivateItemTest : IClassFixture<ItemFixture>
    {
        private readonly ItemFixture _fixture;
        private readonly HttpClient _client;
        private const long ItemId = 3;
        private readonly StringContent _data;

        public ActivateItemTest(ItemFixture itemFixture)
        {
            _fixture = itemFixture;
            _client = _fixture.HttpClient;
            var activateRequest = new PublishItemRequest
            {
                Expiration = DateTime.Now.AddMinutes(10)
            };
            _data = Utils.ConvertRequestData(activateRequest, Utils.ContentType.Json);
        }

        [Fact]
        public async Task Activate_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{ItemId}", _data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Activate_ReturnsForbidden_WhenUserIsNotTheOwner()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SecondSimpleUserJwt);

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{ItemId}", _data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Activate_ReturnsSuccessAndActivatesItem_WhenUserIsTheOwner()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SimpleMainUserJwt);

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{ItemId}", _data);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
