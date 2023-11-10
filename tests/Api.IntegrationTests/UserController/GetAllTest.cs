using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    [Collection("User Collection")]
    public class GetAllTest
    {
        private readonly HttpClient _client;

        public GetAllTest()
        {
            _client = UserFixture.HttpClient;
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserFixture.AdminJwt);

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/user/all");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            var simpleUserJwt = await Utils.LoginUser(_client, UserFixture.SimpleUserCredentials);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", simpleUserJwt.AccessToken);

            // Act
            var getAllResponse = await _client.GetAsync($"{Utils.BaseUrl}/user/all");

            // Assert
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsUnauthorized_WhenAuthorizationIsInvalid()
        {
            // Arrange
            const string jwt = "InvalidJwt";

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await _client.GetAsync($"{Utils.BaseUrl}/user/all");

            // Assert
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsOk_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserFixture.AdminJwt);

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/user/usernames");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            var simpleUserJwt = await Utils.LoginUser(_client, UserFixture.SimpleUserCredentials);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", simpleUserJwt.AccessToken);

            // Act
            var getAllResponse = await _client.GetAsync($"{Utils.BaseUrl}/user/usernames");

            // Assert
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsUnauthorized_WhenAuthorizationIsInvalid()
        {
            // Arrange
            const string jwt = "InvalidJwt";

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await _client.GetAsync($"{Utils.BaseUrl}/user/all");

            // Assert
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
