﻿using System.Net.Http.Headers;
using contracts.Requests.Item;
using Newtonsoft.Json;
using System.Text;
using contracts.Responses.Item;
using FluentAssertions;
using System.Net;

namespace Api.IntegrationTests.ItemController
{
    [Collection("Item Collection")]
    public class CreateItemTest : IClassFixture<ItemFixture>
    {
        private readonly HttpClient _client;

        public CreateItemTest()
        {
            _client = ItemFixture.HttpClient;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ItemFixture.SimpleUserJwt);
        }

        [Fact]
        public async Task CreateItem_ReturnsItem_WhenInputIsCorrectWithoutImage()
        {
            // Arrange
            var itemData = new AddItemRequest
            {
                Name = "test item",
                FirstBid = 50,
                CategoryIds = new List<int> { 1, 2 },
                Description = "test item description"
            };

            var addItemBody = new StringContent(JsonConvert.SerializeObject(itemData), Encoding.UTF8, "application/json");


            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}item", addItemBody);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var createdItem = JsonConvert.DeserializeObject<AddItemResponse>(responseString);

            createdItem.Should().NotBeNull();

            createdItem!.Name.Should().Be(itemData.Name);

            var adminJwt = await Utils.LoginAdmin(_client);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);

            await _client.DeleteAsync($"{Utils.BaseUrl}item/{createdItem.ItemId}");
        }

        [Fact]
        public async Task CreateItem_ReturnsBadRequest_WhenCategoriesDoNotExist()
        {
            // Arrange
            var itemData = new AddItemRequest
            {
                Name = "test item",
                FirstBid = 50,
                CategoryIds = new List<int> { 100, 200 },
                Description = "test item description"
            };

            var addItemBody = new StringContent(JsonConvert.SerializeObject(itemData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}item", addItemBody);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateItem_ReturnsBadRequest_WhenFirstBidIsNotPositive()
        {
            // Arrange
            var itemData = new AddItemRequest
            {
                Name = "test item",
                FirstBid = -2,
                CategoryIds = new List<int> { 100, 200 },
                Description = "test item description"
            };

            var addItemBody = new StringContent(JsonConvert.SerializeObject(itemData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}item", addItemBody);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public static IEnumerable<object[]> IncompleteItemData()
        {
            yield return new object[] { new AddItemRequest
            {
                FirstBid = 50,
                CategoryIds = new List<int> { 1, 2 },
                Description = "test item description"
            } };
            yield return new object[] { new AddItemRequest
            {
                CategoryIds = new List<int> { 1, 2 },
                Description = "test item description"
            } };
            yield return new object[] { new AddItemRequest
            {
                Description = "test item description"
            } };
            yield return new object[] { new AddItemRequest
            {
            } };
        }

        [Theory]
        [MemberData(nameof(IncompleteItemData))]
        public async Task CreateItem_ReturnsBadRequest_WhenInputIsIncomplete(AddItemRequest item)
        {
            // Arrange
            var json = JsonConvert.SerializeObject(item);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{Utils.BaseUrl}item", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
