namespace contracts.Requests.Bid
{
    public class BidStep
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal Step { get; set; }

        public BidStep(decimal minPrice, decimal maxPrice, decimal step)
        {
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Step = step;
        }
    }
}
