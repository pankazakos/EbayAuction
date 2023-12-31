﻿namespace contracts.Endpoints
{
    public static class ItemEndpoints
    {
        public const string BaseUrl = "api/Item";

        public const string Create = "";

        public const string Search = "";

        public const string GetImage = "{guid}";

        public const string Categories = "Categories/{id:long}";

        public const string Inactive = "Inactive";

        public const string Active = "Active";

        public const string Bidden = "Bidden";

        public const string GetById = "{id:long}";

        public const string Activate = "Activate/{id:long}";

        public const string Edit = "Edit/{id:long}";

        public const string Delete = "{id:long}";
    }
}
