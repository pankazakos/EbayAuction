using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string PasswordSalt { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public DateTime DateJoined { get; init; }
        [MaxLength(255)] public string Email { get; init; } = string.Empty;

        [MaxLength(12), MinLength(12)] public string Country { get; set; } = string.Empty;
        [MaxLength(100)] public string Location { get; set; } = string.Empty;

        public bool IsSuperuser { get; init; } = false;
        public bool IsActive { get; set; } = true;

        // navigation properties
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
