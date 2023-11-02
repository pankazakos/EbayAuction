using System.Text;
using Newtonsoft.Json;
using contracts.Responses.User;
using contracts.Requests.User;
using System.Net.Http.Headers;
using System.Net;

namespace Api.IntegrationTests.UserController
{
    public class CreateUserTest : IDisposable
    {
        private readonly HttpClient _client;
        private RegisterUserResponse? _returnedUser;
        private readonly string _adminJwt;

        public CreateUserTest()
        {
            _client = new HttpClient();
            _adminJwt = Utils.LoginAdmin(_client).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task Create_ReturnsUser_WhenUserIsCreated()
        {
            // Arrange
            var user = new RegisterUserRequest
            {
                Username = "testUser",
                Password = "testPassword",
                FirstName = "Test",
                LastName = "User",
                Email = "test@user.com",
                Country = "Test Country",
                Location = "Test Location"
            };

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}user", data);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            _returnedUser = JsonConvert.DeserializeObject<RegisterUserResponse>(responseString);

            // Assert
            Assert.NotNull(_returnedUser);

            Assert.Equal(user.Username, _returnedUser.UserName);
        }

        public static IEnumerable<object[]> IncompleteUserData()
        {
            yield return new object[] { new RegisterUserRequest
            {
                Password = "testPassword",
                FirstName = "Test",
                LastName = "User",
                Email = "test@user.com",
                Country = "Test Country",
                Location = "Test Location"
            } };
            yield return new object[] { new RegisterUserRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@user.com",
                Country = "Test Country",
                Location = "Test Location"
            } };
            yield return new object[] { new RegisterUserRequest
            {
                LastName = "User",
                Email = "test@user.com",
                Country = "Test Country",
                Location = "Test Location"
            } };
            yield return new object[] { new RegisterUserRequest
            {
                Email = "test@user.com",
                Country = "Test Country",
                Location = "Test Location"
            } };
            yield return new object[] { new RegisterUserRequest
            {
                Country = "Test Country",
                Location = "Test Location"
            } };
            yield return new object[] { new RegisterUserRequest
            {
                Location = "Test Location"
            } };
        }

        [Theory]
        [MemberData(nameof(IncompleteUserData))]
        public async Task Create_ReturnsBadRequest_WhenUserInfoIsIncomplete(RegisterUserRequest user)
        {
            // Arrange
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}user", data);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        public void Dispose()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminJwt);

            if (_returnedUser != null)
            {
                var userId = _returnedUser.Id;
                _client.DeleteAsync($"{Utils.BaseUrl}user/{userId}").GetAwaiter().GetResult();
            }
        }
    }

}
