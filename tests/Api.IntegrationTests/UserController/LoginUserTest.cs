using System.Net;
using contracts.Requests.User;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using contracts.Responses.User;

namespace Api.IntegrationTests.UserController
{
    public class LoginUserTest
    {
        private readonly HttpClient _client;

        public LoginUserTest()
        {
            _client = new HttpClient();
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

            Assert.NotNull(jwtResponse);
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
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
