namespace webapi.Contracts.Requests
{
    public class UserCredentialsRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
