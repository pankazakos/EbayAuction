namespace contracts.Requests.Bid
{
    public static class OrderType
    {
        public const string Ascending = "ascending";
        public const string Descending = "descending";
    }

    public static class OrderByOption
    {
        public const string Time = "time";
        public const string Amount = "amount";
    }

    public class GetBidsOrderOptions
    {
        public string OrderType { get; init; } = Bid.OrderType.Ascending;

        public string OrderByOption { get; init; } = Bid.OrderByOption.Time;

        public void Validate()
        {
            if(OrderType != Bid.OrderType.Ascending && OrderType != Bid.OrderType.Descending)
            {
                throw new ArgumentException("bid order type must be either ascending or descending string literal");
            }

            if(OrderByOption != Bid.OrderByOption.Time && OrderByOption != Bid.OrderByOption.Amount) 
            {
                throw new ArgumentException("bid order by option must be either time or amount string literal");
            }
        }
    }
}
