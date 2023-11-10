using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    [Collection("User Collection")]
    public class DeleteUserTest
    {
        private readonly HttpClient _client;

        public DeleteUserTest()
        {
            _client = UserFixture.HttpClient;
            var adminJwt = UserFixture.AdminJwt;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = UserFixture.IdUserToRemove; // TestUserToRemove (id = 3)

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/user/{userId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            const int invalidUserId = 9999;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/user/{invalidUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
