namespace webapi.Contracts.Endpoints
{
    public class ItemEndpoints
    {
        public const string BaseUrl = "api/Item";

        public const string Create = "";

        public const string All = "All";

        public const string Inactive = "Inactive";

        public const string Active = "Active";

        public const string Bidden = "Bidden";

        public const string GetById = "{id:long}";

        public const string Activate = "Activate/{id:long}";

        public const string Bid = "bid/{id:long}";
    }
}
