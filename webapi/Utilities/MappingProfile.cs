using AutoMapper;
using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define your mappings here
            CreateMap<User, NoPasswordUserResponse>();
            CreateMap<User, RegisterUserResponse>();
        }
    }

}
