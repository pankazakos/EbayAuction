﻿using System.Text;
using Newtonsoft.Json;
using contracts.Responses.User;
using contracts.Requests.User;
using System.Net.Http.Headers;

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

            var response = await _client.PostAsync($"{Utils.BaseUrl}user", data);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            _returnedUser = JsonConvert.DeserializeObject<RegisterUserResponse>(responseString);

            Assert.NotNull(_returnedUser);
            if (_returnedUser != null)
            {
                Assert.Equal(user.Username, _returnedUser.UserName);
            }
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