namespace contracts.Requests.Bid
{
    public class AddBidRequest : IAppRequest
    {
        public long ItemId { get; init; }

        public float Amount { get; init; }

        public void Validate()
        {
           RequestUtils.EnsurePositiveNumber(nameof(ItemId), ItemId);
           RequestUtils.EnsurePositiveNumber(nameof(Amount), Amount);
        }
    }
}
