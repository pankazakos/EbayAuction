namespace contracts.Responses.bid
{
    public static class AuctionStatusType
    {
        public const string Active = "active";

        public const string Expired = "expired";
    }

    public class ExtendedBidInfo : BasicBidResponse
    {
        public string Seller { get; init; } = string.Empty;
        public string ItemTitle { get; init; } = string.Empty;
        public string AuctionStatus { get; init; } = AuctionStatusType.Active;
    }
}
