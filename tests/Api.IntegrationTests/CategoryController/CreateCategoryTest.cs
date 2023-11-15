using System.Net;
using System.Net.Http.Headers;
using contracts.Requests.Category;
using contracts.Responses.Category;
using FluentAssertions;

namespace Api.IntegrationTests.CategoryController
{
    public class CreateCategoryTest : IClassFixture<CategoryFixture>
    {
        private readonly CategoryFixture _fixture;
        private readonly HttpClient _client;
        private readonly AddCategoryRequest _category;
        private readonly StringContent _categoryData;
        private readonly string _url;

        public CreateCategoryTest(CategoryFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            _category = new AddCategoryRequest
            {
                Name = "test category"
            };
            _categoryData = Utils.ConvertRequestData(_category, Utils.ContentType.Json);
            _url = $"{Utils.BaseUrl}/category";
        }

        [Fact]
        public async Task Create_ReturnsCreatedAndCreatesCategory_WhenAdminMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.AdminJwt);

            // Act
            var response = await _client.PostAsync(_url, _categoryData);
            var responseData = await Utils.ConvertResponseData<BasicCategoryResponse>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            responseData.Should().NotBeNull();

            responseData!.Name.Should().Be(_category.Name);
        }

        [Fact]
        public async Task Create_ReturnsForbidden_WhenSimpleUserMakesRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _fixture.SimpleMainUserJwt);

            // Act
            var response = await _client.PostAsync(_url, _categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsync(_url, _categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

    }
}
