namespace contracts.Requests.User
{
    public class RegisterUserRequest : IAppRequest
    {
        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Country { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public void Validate()
        {
            RequestUtils.EnsureStringContent(nameof(Username), Username);
            RequestUtils.EnsureStringContent(nameof(Password), Password);
            RequestUtils.EnsureStringContent(nameof(FirstName), FirstName);
            RequestUtils.EnsureStringContent(nameof(LastName), LastName);
            RequestUtils.EnsureStringContent(nameof(Email), Email);
            RequestUtils.EnsureStringContent(nameof(Country), Country);
            RequestUtils.EnsureStringContent(nameof(Location), Location);
        }
    }
}
