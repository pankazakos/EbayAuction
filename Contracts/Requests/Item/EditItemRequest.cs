namespace contracts.Requests.Item
{
    public class EditItemRequest
    {
        public string? Name { get; init; } = string.Empty;

        public List<int>? CategoryIds { get; init; } = new();

        public decimal? BuyPrice { get; init; }

        public decimal? FirstBid { get; init; }

        public string? Description { get; init; } = string.Empty;
    }
}
