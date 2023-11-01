using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Api.Tests.Integration;

namespace Api.Tests.Integration.UserController
{
    public class CreateUserTest : IDisposable
    {
        private readonly HttpClient _client;
        private dynamic? _returnedUser;

        public CreateUserTest()
        {
            _client = new HttpClient();
        }

        [Fact]
        public async Task Create_ReturnsUser_WhenUserIsCreated()
        {
            var user = new
            {
                username = "testUser",
                password = "testPassword",
                firstname = "Test",
                lastname = "User",
                email = "test@user.com",
                country = "Test Country",
                location = "Test Location"
            };

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{Utils.BaseUrl}user", data);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            _returnedUser = JsonConvert.DeserializeObject<dynamic>(responseString);

            Assert.NotNull(_returnedUser);
            if (_returnedUser != null)
            {
                Assert.Equal(user.username, _returnedUser.userName);
            }
        }

        public void Dispose()
        {
            if (_returnedUser != null)
            {
                var userId = _returnedUser.id;
                var response = _client.DeleteAsync($"{Utils.BaseUrl}user/{userId}").Result;
            }
        }
    }
}