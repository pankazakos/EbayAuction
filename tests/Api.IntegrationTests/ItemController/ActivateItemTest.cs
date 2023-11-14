using System.Net;
using System.Net.Http.Headers;
using contracts.Requests.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    [Collection("Item Collection")]
    public class ActivateItemTest
    {
        private readonly HttpClient _client = ItemFixture.HttpClient;

        [Fact]
        public async Task Activate_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            const long itemId = 3;

            var activateRequest = new PublishItemRequest
            {
                Expiration = DateTime.Now.AddMinutes(10)
            };

            var data = Utils.ConvertRequestData(activateRequest, Utils.ContentType.Json);

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{itemId}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Activate_ReturnsForbidden_WhenUserIsNotTheOwner()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", ItemFixture.SecondSimpleUserJwt);

            const long itemId = 3;

            var activateRequest = new PublishItemRequest
            {
                Expiration = DateTime.Now.AddMinutes(10)
            };

            var data = Utils.ConvertRequestData(activateRequest, Utils.ContentType.Json);

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{itemId}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Activate_ReturnsSuccessAndActivatesItem_WhenUserIsTheOwner()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", ItemFixture.SimpleMainUserJwt);

            const long itemId = 3;

            var activateRequest = new PublishItemRequest
            {
                Expiration = DateTime.Now.AddMinutes(10)
            };

            var data = Utils.ConvertRequestData(activateRequest, Utils.ContentType.Json);

            // Act
            var response = await _client.PutAsync($"{Utils.BaseUrl}/item/activate/{itemId}", data);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
