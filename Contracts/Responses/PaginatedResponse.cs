namespace contracts.Responses
{
    public class PaginatedResponse<T> where T : IEntityResponse
    {
        public IEnumerable<T> CastEntities { get; init; } = new List<T>();
        public int Total { get; init; }
        public int Page { get; init; }
        public int Limit { get; init; }
    }
}
