using contracts.Responses.User;
using contracts.Requests.User;
using System.Net;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    public class CreateUserTest : IClassFixture<UserFixture>
    {
        private readonly UserFixture _fixture;
        private readonly HttpClient _client;

        public CreateUserTest(UserFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
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

            var data = Utils.ConvertRequestData(user, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}/user", data);
            response.EnsureSuccessStatusCode();

            var returnedUser = await Utils.ConvertResponseData<RegisterUserResponse>(response);
            
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
            var data = Utils.ConvertRequestData(user, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}/user", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

}
