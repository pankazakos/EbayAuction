using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    [Collection("Item Collection")]
    public class DeleteItemTest
    {
        private readonly HttpClient _client = ItemFixture.HttpClient;


        [Fact]
        public async Task Delete_ReturnsReturnsNoContent_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ItemFixture.AdminJwt);
            const long itemId = 4;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsForbidden_WhenUserMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ItemFixture.SimpleMainUserJwt);
            const long itemId = 4;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            const long itemId = 4;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
