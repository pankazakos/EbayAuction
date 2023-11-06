using Newtonsoft.Json;
using contracts.Responses.User;
using System.Net.Http.Headers;
using System.Net;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    public class GetUserTest
    {
        private readonly HttpClient _client;
        private readonly string _adminJwt;
        public GetUserTest()
        {
            _client = new HttpClient();
            _adminJwt = Utils.LoginAdmin(_client).GetAwaiter().GetResult();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminJwt);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            // Arrange
            const int userId = 1;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<BasicUserResponse>(responseString);

            returnedUser.Should().NotBeNull();

            returnedUser!.Id.Should().Be(userId);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            const int userId = 9999;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsUser_WhenUserExists()
        {
            // Arrange
            const string username = "panagiotis";

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{username}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<BasicUserResponse>(responseString);

            returnedUser.Should().NotBeNull();

            returnedUser!.Username.Should().Be(username);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            const string username = "nonExistingUsername";

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{username}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
