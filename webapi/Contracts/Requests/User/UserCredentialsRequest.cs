namespace webapi.Contracts.Requests.User
{
    public class UserCredentialsRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
