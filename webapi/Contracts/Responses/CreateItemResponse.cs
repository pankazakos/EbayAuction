namespace webapi.Contracts.Responses
{
    public class CreateItemResponse
    {
        public long ItemId { get; init; }

        public string Name { get; set; } = string.Empty;

        public float Currently { get; set; }

        public float? BuyPrice { get; set; }

        public float FirstBid { get; set; }

        public int NumBids { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; } = string.Empty;

        public int SellerId { get; init; }
    }
}
