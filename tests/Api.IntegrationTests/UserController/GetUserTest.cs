using Newtonsoft.Json;
using contracts.Responses.User;
using System.Net.Http.Headers;
using System.Net;

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
            const int userId = 1;

            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{userId}");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<BasicUserResponse>(responseString);

            Assert.NotNull(returnedUser);

            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            const int userId = 9999;

            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{userId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsUser_WhenUserExists()
        {
            const string username = "panagiotis";

            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{username}");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<BasicUserResponse>(responseString);

            Assert.NotNull(returnedUser);

            Assert.Equal(username, returnedUser.Username);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsNotFound_WhenUserDoesNotExist()
        {
            const string username = "nonExistingUsername";

            var response = await _client.GetAsync($"{Utils.BaseUrl}user/{username}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
