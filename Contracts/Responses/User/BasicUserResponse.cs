namespace contracts.Responses.User
{
    public class BasicUserResponse
    {
        public int Id { get; init; }

        public string Username { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public DateTime? LastLogin { get; init; }

        public DateTime DateJoined { get; init; }

        public string Country { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public bool IsSuperuser { get; init; } = false;

        public bool IsActive { get; init; } = true;
    }
}
