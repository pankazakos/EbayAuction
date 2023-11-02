namespace webapi.Contracts.Responses.User
{
    public class PaginatedUserResponse : IUserResponse
    {
        public IEnumerable<BasicUserResponse> Users { get; init; } = new List<BasicUserResponse>();
        public int Total { get; init; }
        public int Page { get; init; }
        public int Limit { get; init; }
    }
}
