namespace contracts.Requests.Bid
{
    public static class BidStepService
    {
        private static readonly List<BidStep> BidSteps;

        static BidStepService()
        {
            BidSteps = new List<BidStep>
            {
                new(0.01m, 0.99m, 0.05m),
                new(1.00m, 4.99m, 0.25m),
                new(5.00m, 24.99m, 0.50m),
                new(25.00m, 99.99m, 1.00m),
                new(100.00m, 249.99m, 2.50m),
                new(250.00m, 499.99m, 5.00m),
                new(500.00m, 999.99m, 10.00m),
                new(1000.00m, 2499.99m, 25.00m),
                new(2500.00m, 4999.99m, 50.00m),
                new(5000.00m, decimal.MaxValue, 100.00m)
            };
        }

        public static bool IsBidAmountValid(decimal currentPrice, decimal bidAmount, bool addBidStep)
        {
            var bidStep = GetBidStep(currentPrice);

            if (addBidStep)
            {
                return bidAmount >= currentPrice + bidStep;
            }

            return bidAmount >= currentPrice;
        }

        public static decimal GetBidStep(decimal currentPrice)
        {
            try
            {
                var range = BidSteps.First(b => currentPrice >= b.MinPrice && currentPrice <= b.MaxPrice);
                return range.Step;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"No bid step found for the current price of {currentPrice:C}");
            }
        }
    }
}
