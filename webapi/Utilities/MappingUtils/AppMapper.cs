using AutoMapper;
using contracts.Responses;
using webapi.Models;

namespace webapi.Utilities.MappingUtils
{
    public static class AppMapper
    {
        public static IEntityResponse MapToResponse<TResponse>(this IModel model, IMapper mapper) where TResponse : IEntityResponse
        {
            return mapper.Map<TResponse>(model);
        }
    }
}
