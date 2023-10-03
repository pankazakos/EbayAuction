namespace webapi.Contracts.Responses
{
    public class AddItemResponse
    {
        public long ItemId { get; init; }

        public string Name { get; init; } = string.Empty;

        public float Currently { get; init; }

        public float? BuyPrice { get; init; }

        public float FirstBid { get; init; }

        public int NumBids { get; init; }

        public bool Active { get; init; }

        public string Description { get; init; } = string.Empty;

        public int SellerId { get; init; }
    }
}
