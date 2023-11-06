using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    public class DeleteUserTest
    {
        private readonly HttpClient _client;

        public DeleteUserTest()
        {
            _client = new HttpClient();
            var adminJwt = Utils.LoginAdmin(_client).GetAwaiter().GetResult();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            const int invalidUserId = 9999;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}user/{invalidUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
