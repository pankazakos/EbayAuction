using AutoMapper;
using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Contracts.Mapping
{
    public static class AppMapper
    {
        public static IEntityResponse MapToResponse<TResponse>(this IModel model, IMapper mapper) where TResponse : IEntityResponse
        {
            return mapper.Map<TResponse>(model);
        }
    }
}
