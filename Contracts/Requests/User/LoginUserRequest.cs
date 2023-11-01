namespace Contracts.Requests.User
{
    public class LoginUserRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
