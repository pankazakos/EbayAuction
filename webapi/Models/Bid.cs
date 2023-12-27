namespace webapi.Models
{
    public class Bid : IModel
    {
        public long BidId { get; init; }

        public DateTime Time { get; init; }

        public decimal Amount { get; init; }

        public long ItemId { get; init; }

        public int BidderId { get; init; }


        public Item Item { get; set; } = new();

        public User Bidder { get; set; } = new();
    }

}
