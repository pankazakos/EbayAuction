namespace contracts.Requests.User
{
    public class LoginUserRequest : IAppRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public void Validate()
        {
            RequestUtils.EnsureStringContent(nameof(Username), Username);
            RequestUtils.EnsureStringContent(nameof(Password), Password);
        }
    }
}
