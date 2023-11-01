namespace Contracts.Requests.User
{
    public class RegisterUserRequest
    {
        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Country { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;
    }
}
