using webapi.Models;

namespace webapi.Contracts.Responses
{
    public class PaginatedResponse<T> where T : IEntityResponse
    {
        public IEnumerable<T> CastEntities { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
