namespace contracts.Requests.Bid
{
    public class AddBidRequest : IRequest
    {
        public long ItemId { get; init; }

        public void Validate()
        {
           RequestUtils.EnsurePositiveNumber(nameof(ItemId), ItemId);
        }
    }
}
