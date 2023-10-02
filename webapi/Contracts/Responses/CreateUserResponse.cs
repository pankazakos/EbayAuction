namespace webapi.Contracts.Responses
{
    public class CreateUserResponse : IResponse
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime DateJoined { get; set; }

        public string Country { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public bool IsSuperuser { get; set; }
    }
}
