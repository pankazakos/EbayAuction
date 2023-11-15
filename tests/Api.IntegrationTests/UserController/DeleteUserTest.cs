using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    public class DeleteUserTest : IClassFixture<UserFixture>
    {
        private readonly UserFixture _fixture;
        private readonly HttpClient _client;

        public DeleteUserTest(UserFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            var adminJwt = _fixture.AdminJwt;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = 2; // TestUser (id = 2)

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
