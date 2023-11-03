namespace contracts.Responses.User
{
    public class RegisterUserResponse : IUserResponse
    {
        public long Id { get; init; }

        public string UserName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public DateTime DateJoined { get; init; }

        public string Country { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public bool IsSuperuser { get; init; }
    }
}
