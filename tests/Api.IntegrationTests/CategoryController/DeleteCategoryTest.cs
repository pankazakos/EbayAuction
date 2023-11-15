using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Api.IntegrationTests.CategoryController
{
    public class DeleteCategoryTest : IClassFixture<CategoryFixture>
    {
        private readonly CategoryFixture _fixture;
        private readonly HttpClient _client;
        private const int CategoryId = 1;
        private readonly string _url;

        public DeleteCategoryTest(CategoryFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            _url = $"{Utils.BaseUrl}/category/{CategoryId}";
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.AdminJwt);

            // Act
            var response = await _client.DeleteAsync(_url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.AdminJwt);

            const int categoryId = 9999;

            // Act
            var response = await _client.DeleteAsync($"{Utils.BaseUrl}/category/{categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SimpleMainUserJwt);

            // Act
            var response = await _client.DeleteAsync(_url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.DeleteAsync(_url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
