using System.Net;
using contracts.Responses;
using contracts.Responses.Item;
using FluentAssertions;

namespace Api.IntegrationTests.ItemController
{
    public class GetItemsTest : IClassFixture<ItemFixture>
    {
        private readonly ItemFixture _fixture;
        private readonly HttpClient _client;

        public GetItemsTest(ItemFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
        }

        [Fact]
        public async Task GetItemById_ReturnsItem_WhenIdExists()
        {
            // Arrange
            const long itemId = 1;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/{itemId}");

            var item = await Utils.ConvertResponseData<BasicItemResponse>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            item.Should().NotBeNull();

            item!.ItemId.Should().Be(itemId);
        }

        [Fact]
        public async Task GetItemById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            const long itemId = 9999;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllPaged_ReturnsDefaultNumberOfItems_WhenNoParametersAreGiven()
        {
            // Arrange
            //const int page = 1;
            const int limit = 10;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item");

            var responseData = await Utils.ConvertResponseData<PaginatedResponse<BasicItemResponse>>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(responseData);

            var items = responseData.CastEntities.ToList();

            items.Count.Should().BeLessOrEqualTo(limit);
            items.FirstOrDefault()!.ItemId.Should().Be(1);
        }

        [Fact]
        public async Task GetAllPaged_ReturnsCorrectNumberOfItems_WhenPagingParametersAreGiven()
        {
            // Arrange
            const int page = 2;
            const int limit = 1;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item?page={page}&limit={limit}");

            var responseData = await Utils.ConvertResponseData<PaginatedResponse<BasicItemResponse>>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(responseData);

            var items = responseData.CastEntities.ToList();

            items.Count.Should().BeLessOrEqualTo(limit);
            items.FirstOrDefault()!.ItemId.Should().Be((page - 1) * limit + 1);
        }

        [Fact]
        public async Task GetAllPaged_ReturnsEmptyList_WhenPageDoesNotExist()
        {
            // Arrange
            const int page = 9999;
            const int limit = 10;

            // Act
            var response = await _client.GetAsync($"{Utils.BaseUrl}/item?page={page}&limit={limit}");

            var responseData = await Utils.ConvertResponseData<PaginatedResponse<BasicItemResponse>>(response);

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.NotNull(responseData);

            var items = responseData.CastEntities.ToList();

            items.Count.Should().Be(0);
        }
    }
}
