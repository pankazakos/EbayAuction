using System.Net;
using contracts.Responses.bid;
using FluentAssertions;

namespace Api.IntegrationTests.BidController
{
    public class GetItemBidsTest : IClassFixture<BidFixture>
    {
        private readonly BidFixture _fixture;
        private readonly HttpClient _client;
        private readonly string _url;

        public GetItemBidsTest(BidFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            _url = $"{Utils.BaseUrl}/bid";
        }


        [Fact]
        public async Task GetItemBids_ReturnsCorrectNumberOfBids_WhenBidsExist()
        {
            // Arrange
            const long itemId = 1;
            const int numBids = 2;

            // Act
            var response = await _client.GetAsync($"{_url}?itemId={itemId}");

            var responseData = (await Utils.ConvertResponseData<IEnumerable<BasicBidResponse>>(response))?.ToList();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.NotNull(responseData);

            responseData.Count.Should().Be(numBids);
        }


        [Fact]
        public async Task GetItemBids_ReturnsEmptyList_WhenBidsDoNotExist()
        {
            // Arrange
            const long itemId = 2;

            // Act
            var response = await _client.GetAsync($"{_url}?itemId={itemId}");

            var responseData = await Utils.ConvertResponseData<IEnumerable<BasicBidResponse>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.NotNull(responseData);

            responseData.Count().Should().Be(0);
        }


        [Fact]
        public async Task GetItemBids_ReturnsBadRequest_WhenItemDoesNotExist()
        {
            // Arrange
            const long itemId = 9999;

            // Act
            var response = await _client.GetAsync($"{_url}?itemId={itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
