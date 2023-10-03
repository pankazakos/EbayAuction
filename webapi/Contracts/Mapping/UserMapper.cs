using webapi.Contracts.Responses;
using webapi.Models;
using AutoMapper;

namespace webapi.Contracts.Mapping
{
    public static class UserMapper
    {
        public static NoPasswordUserResponse MapToNoPasswordUserResponse(this User user, IMapper mapper)
        {
            return mapper.Map<NoPasswordUserResponse>(user);
        }

        public static RegisterUserResponse MapToRegisterUserResponse(this User user, IMapper mapper)
        {
            return mapper.Map<RegisterUserResponse>(user);
        }
    }
}
