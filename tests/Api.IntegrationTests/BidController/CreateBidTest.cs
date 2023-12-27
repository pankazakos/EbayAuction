using System.Net;
using System.Net.Http.Headers;
using contracts.Requests.Bid;
using contracts.Responses.bid;
using FluentAssertions;

namespace Api.IntegrationTests.BidController
{
    public class CreateBidTest : IClassFixture<BidFixture>
    {
        private readonly BidFixture _fixture;
        private readonly HttpClient _client;
        private readonly string _url;

        public CreateBidTest(BidFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.HttpClient;
            _url = $"{Utils.BaseUrl}/bid";
        }


        [Fact]
        public async Task Create_ReturnsCreatedAndCreatesBid_WhenBidIsValid()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SecondaryUserJwt);

            var bid = new AddBidRequest
            {
                ItemId = 1,
                Amount = 100.4m
            };

            var data = Utils.ConvertRequestData(bid, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync(_url, data);

            var returnedBid = await Utils.ConvertResponseData<BasicBidResponse>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            returnedBid.Should().NotBeNull();

            returnedBid!.BidderId.Should().Be(3); // Secondary User (id = 3)
        }


        [Fact]
        public async Task Create_ReturnsBadRequest_WhenBidAmountIsNotEnough()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SecondaryUserJwt);

            var bid = new AddBidRequest
            {
                ItemId = 1,
                Amount = 30
            };

            var data = Utils.ConvertRequestData(bid, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync(_url, data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Create_ReturnsBadRequest_WhenItemDoesNotExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _fixture.SecondaryUserJwt);

            var bid = new AddBidRequest
            {
                ItemId = 9999,
                Amount = 100
            };

            var data = Utils.ConvertRequestData(bid, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync(_url, data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenNoAuthorizationIsProvided()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            var bid = new AddBidRequest
            {
                ItemId = 1,
                Amount = 100
            };

            var data = Utils.ConvertRequestData(bid, Utils.ContentType.Json);

            // Act
            var response = await _client.PostAsync(_url, data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
