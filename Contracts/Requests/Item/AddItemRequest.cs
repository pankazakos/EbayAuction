namespace contracts.Requests.Item
{
    public class AddItemRequest : IAppRequest
    {
        public string Name { get; init; } = string.Empty;

        public List<int> CategoryIds { get; init; } = new();

        public decimal? BuyPrice { get; init; }

        public decimal FirstBid { get; init; }

        public string Description { get; init; } = string.Empty;

        public void Validate()
        {
            RequestUtils.EnsureStringContent(nameof(Name), Name);
            RequestUtils.EnsurePositiveNumber(nameof(CategoryIds), CategoryIds.Count);
            if (BuyPrice.HasValue)
            {
                RequestUtils.EnsurePositiveNumber(nameof(BuyPrice), BuyPrice.Value);
            }
            RequestUtils.EnsurePositiveNumber(nameof(FirstBid), FirstBid);
            RequestUtils.EnsureStringContent(nameof(Description), Description);
        }
    }
}
