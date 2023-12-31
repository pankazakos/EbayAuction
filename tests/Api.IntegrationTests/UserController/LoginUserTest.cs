﻿using System.Net;
using contracts.Requests.User;
using Newtonsoft.Json;
using System.Text;
using contracts.Responses.User;
using FluentAssertions;

namespace Api.IntegrationTests.UserController
{
    public class LoginUserTest : IClassFixture<UserFixture>
    {
        private readonly UserFixture _fixture;
        private readonly HttpClient _client;

        public LoginUserTest(UserFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
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

            var data = Utils.ConvertRequestData(userCredentials, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}/user/login", data);

            // Assert
            response.EnsureSuccessStatusCode();

            var jwtResponse = Utils.ConvertResponseData<LoginUserResponse>(response);

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

            var data = Utils.ConvertRequestData(userCredentials, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}/user/login", data);

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

            var data = Utils.ConvertRequestData(userCredentials, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}/user/login", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
