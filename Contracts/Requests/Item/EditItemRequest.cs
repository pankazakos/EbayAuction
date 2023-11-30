﻿namespace contracts.Requests.Item
{
    public class EditItemRequest
    {
        public string? Name { get; init; } = string.Empty;

        public List<int>? CategoryIds { get; init; } = new();

        public float? BuyPrice { get; init; }

        public float? FirstBid { get; init; }

        public string? Description { get; init; } = string.Empty;
    }
}