namespace contracts.Responses.bid
{
    public class BasicBidResponse : IBidResponse
    {
        public long BidId { get; init; }

        public DateTime Time { get; init; }

        public decimal Amount { get; init; }

        public long ItemId { get; init; }

        public int BidderId { get; init; }
    }
}
