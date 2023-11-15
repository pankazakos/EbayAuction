using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class DeleteItemTest : IClassFixture<ItemFixture>
    {
        private readonly ItemFixture _fixture;
        private readonly HttpClient _client;

        public DeleteItemTest(ItemFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
        }

        [Fact]
        public async Task Delete_ReturnsReturnsNoContent_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.AdminJwt);
            const long itemId = 1;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsForbidden_WhenUserIsNotOwnerOfItem()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.SecondSimpleUserJwt);
            const long itemId = 2;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenItemOwnerMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _fixture.SimpleMainUserJwt);
            const long itemId = 2;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            const long itemId = 2;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
