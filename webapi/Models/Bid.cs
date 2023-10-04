namespace webapi.Models
{
    public class Bid : IModel
    {
        public long BidId { get; init; }
        public DateTime Time { get; init; }
        public float Amount { get; init; }
        public long ItemId { get; init; }
        public int BidderId { get; init; }

        // Navigation properties
        public Item Item { get; set; } = new();
        public User Bidder { get; set; } = new();
    }

}
