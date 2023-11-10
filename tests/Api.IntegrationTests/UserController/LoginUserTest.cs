using System.Net;
using contracts.Requests.User;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using contracts.Responses.User;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    [Collection("User Collection")]
    public class LoginUserTest
    {
        private readonly HttpClient _client;

        public LoginUserTest()
        {
            _client = UserFixture.HttpClient;
        }

        [Fact]
        public async Task LoginUser_ReturnsJwt_WhenCredentialsAreValid()
        {
            // Arrange
            var userCredentials = new LoginUserRequest
            {
                Username = "admin",
                Password = "admin"
            };

            var json = JsonConvert.SerializeObject(userCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}user/login", data);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var jwtResponse = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            jwtResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginUser_ReturnsBadRequest_WhenCredentialsAreInvalid()
        {
            // Arrange
            var userCredentials = new LoginUserRequest
            {
                Username = "admin",
                Password = "NonExistingPassword"
            };

            var json = JsonConvert.SerializeObject(userCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}user/login", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LoginUser_ReturnsNotFound_WhenCredentialsAreInvalid()
        {
            // Arrange
            var userCredentials = new LoginUserRequest
            {
                Username = "NonExistingUsername",
                Password = "NonExistingPassword"
            };

            var json = JsonConvert.SerializeObject(userCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}user/login", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
