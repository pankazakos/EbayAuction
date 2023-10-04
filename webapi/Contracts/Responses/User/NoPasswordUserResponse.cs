using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.Contracts.Responses.User
{
    public class NoPasswordUserResponse : IUserResponse
    {
        public int Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateTime? LastLogin { get; init; }
        public DateTime DateJoined { get; init; }
        [MaxLength(255)] public string Email { get; init; } = string.Empty;

        [MaxLength(12), MinLength(12)] public string Country { get; init; } = string.Empty;
        [MaxLength(100)] public string Location { get; init; } = string.Empty;

        public bool IsSuperuser { get; init; } = false;
        public bool IsActive { get; init; } = true;

        // navigation properties
        public ICollection<Bid> Bids { get; init; } = new List<Bid>();
        public ICollection<Models.Item> Items { get; init; } = new List<Models.Item>();
    }
}
