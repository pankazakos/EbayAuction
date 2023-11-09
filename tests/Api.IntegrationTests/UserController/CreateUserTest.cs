using System.Text;
using Newtonsoft.Json;
using contracts.Responses.User;
using contracts.Requests.User;
using System.Net;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    [Collection("User collection")]
    public class CreateUserTest
    {
        private readonly HttpClient _client;

        public CreateUserTest()
        {
            _client = UserFixture.HttpClient;
        }

        [Fact]
        public async Task Create_ReturnsUser_WhenUserIsCreated()
        {
            // Arrange
            var user = new RegisterUserRequest
            {
                Username = "RegisterTestUser",
                Password = "password",
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
            var returnedUser = JsonConvert.DeserializeObject<RegisterUserResponse>(responseString);
            
            // Assert
            returnedUser.Should().NotBeNull();

            user.Username.Should().Be(returnedUser!.UserName);
        }

        public static IEnumerable<object[]> IncompleteUserData()
        {
            yield return new object[] { new RegisterUserRequest
            {
                Password = "password",
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
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

}
