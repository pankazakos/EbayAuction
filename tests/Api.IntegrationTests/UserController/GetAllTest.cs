using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using contracts.Requests.User;
using contracts.Responses.User;

namespace Api.IntegrationTests.UserController
{
    public class GetAllTest
    {
        private readonly HttpClient _client;

        public GetAllTest()
        {
            _client = new HttpClient();
            var adminJwt = Utils.LoginAdmin(_client).GetAwaiter().GetResult();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenAdminMakesRequest()
        {
            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/all");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            var userCredentials = new LoginUserRequest
            {
                Username = "panagiotis",
                Password = "123"
            };

            var tempClient = new HttpClient();

            var json = JsonConvert.SerializeObject(userCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await tempClient.PostAsync($"{Utils.BaseUrl}user/login", data);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            Assert.NotNull(responseBody);

            var jwt = responseBody.AccessToken;

            tempClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await tempClient.GetAsync($"{Utils.BaseUrl}user/all");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, getAllResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsUnauthorized_WhenAuthorizationIsInvalid()
        {
            // Arrange
            var tempClient = new HttpClient();

            const string jwt = "InvalidJwt";

            tempClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await tempClient.GetAsync($"{Utils.BaseUrl}user/all");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, getAllResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsOk_WhenAdminMakesRequest()
        {
            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}user/usernames");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            var userCredentials = new LoginUserRequest
            {
                Username = "panagiotis",
                Password = "123"
            };

            var tempClient = new HttpClient();

            var json = JsonConvert.SerializeObject(userCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await tempClient.PostAsync($"{Utils.BaseUrl}user/login", data);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            Assert.NotNull(responseBody);

            var jwt = responseBody.AccessToken;

            tempClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await tempClient.GetAsync($"{Utils.BaseUrl}user/usernames");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, getAllResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllUsernames_ReturnsUnauthorized_WhenAuthorizationIsInvalid()
        {
            // Arrange
            var tempClient = new HttpClient();

            var jwt = "InvalidJwt";

            tempClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Act
            var getAllResponse = await tempClient.GetAsync($"{Utils.BaseUrl}user/all");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, getAllResponse.StatusCode);
        }
    }
}
