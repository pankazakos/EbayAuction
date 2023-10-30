using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Item : IModel
    {
        [Key]
        public long ItemId { get; init; }

        public string Name { get; set; } = string.Empty;

        public float Currently { get; set; }

        public float? BuyPrice { get; set; }

        public float FirstBid { get; set; }

        public int NumBids { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Ends { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; } = string.Empty;

        public int SellerId { get; init; }

        public Guid ImageGuid { get; set; }


        public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<Bid> Bids { get; set; } = new List<Bid>();

        public User Seller { get; set; } = new();
    }

}
