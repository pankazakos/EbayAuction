using AutoMapper;
using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Contracts.Mapping
{
    public static class ItemMapper
    {
        public static AddItemResponse MapToCreateItemResponse(Item item, IMapper mapper)
        {
            return mapper.Map<AddItemResponse>(item);
        }
    }
}
