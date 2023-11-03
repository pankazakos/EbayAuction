namespace contracts.Responses.Item
{
    public class BasicItemResponse : IItemResponse
    {
        public long ItemId { get; init; }

        public string Name { get; init; } = string.Empty;

        public float Currently { get; init; }

        public float? BuyPrice { get; init; }

        public float FirstBid { get; init; }

        public int NumBids { get; init; }

        public DateTime? Started { get; init; }

        public DateTime? Ends { get; init; }

        public bool Active { get; init; }

        public string Description { get; init; } = string.Empty;

        public int SellerId { get; init; }

        public string? ImageGuid { get; init; }
    }
}
