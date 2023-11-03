namespace contracts.Responses.bid
{
    public class AddBidResponse : IBidResponse
    {
        public long BidId { get; init; }

        public DateTime Time { get; init; }

        public float Amount { get; init; }

        public long ItemId { get; init; }

        public int BidderId { get; init; }
    }
}
