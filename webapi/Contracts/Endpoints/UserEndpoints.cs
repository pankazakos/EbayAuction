﻿namespace webapi.Contracts.Endpoints
{
    public static class UserEndpoints
    {
        public const string BaseUrl = "api/User";

        public const string All = "All";

        public const string Usernames = "Usernames";

        public const string GetById = "{id:int}";

        public const string GetByUsername = "{username}";

        public const string Delete = "{id:int}";

        public const string Create = "";

        public const string Login = "Login";
    }
}
