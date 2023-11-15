using System.Net;
using contracts.Responses.Category;
using FluentAssertions;

namespace Api.IntegrationTests.CategoryController
{
    public class GetCategoriesTest : IClassFixture<CategoryFixture>
    {
        private readonly HttpClient _client;

        public GetCategoriesTest(CategoryFixture fixture)
        {
            _client = fixture.HttpClient;
        }

        [Fact]
        public async Task GetById_ReturnsCorrectCategory_WhenItExists()
        {
            // Arrange
            const int categoryId = 1;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/Category/{categoryId}");

            var category = await Utils.ConvertResponseData<BasicCategoryResponse>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            category.Should().NotBeNull();

            category!.Id.Should().Be(categoryId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            const int categoryId = 9999;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/Category/{categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList_WhetherTheyExistOrNot()
        {
            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/Category/All");

            var categories = await Utils.ConvertResponseData<IEnumerable<BasicCategoryResponse>>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            categories.Should().NotBeNull();
        }
    }
}
