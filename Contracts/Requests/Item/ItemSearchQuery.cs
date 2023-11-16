namespace contracts.Requests.Item
{
    public class ItemSearchQuery
    {
        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 10;

        public string Title { get; init; } = string.Empty;

        public List<string> Categories { get; init; } = new();

        public int MinPrice { get; init; }

        public int MaxPrice { get; init; }

        public void Validate()
        {
            RequestUtils.EnsureNonNegativeNumber(nameof(Page), Page);
            RequestUtils.EnsureNonNegativeNumber(nameof(Limit), Limit);
            RequestUtils.EnsureNonNegativeNumber(nameof(MinPrice), MinPrice);
            RequestUtils.EnsureNonNegativeNumber(nameof(MaxPrice), MaxPrice);
            if (MinPrice > MaxPrice)
            {
                throw new ArgumentException("Min price cannot be greater than Max Price");
            }
        }
    }
}
