using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Contracts.Mapping
{
    public static class UserMapper
    {
        public static CreateUserResponse MapToCreateUserResponse(User? user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            return new CreateUserResponse
            {
                Id = user.Id,
                UserName = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateJoined = user.DateJoined,
                Country = user.Country,
                Location = user.Location,
                IsSuperuser = user.IsSuperuser
            };
        }
    }
}
