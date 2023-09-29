using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Database
{
    public interface IAuctionContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Item> Items { get; set; }

        DbSet<Category> Categories { get; set; }

        DbSet<Bid> Bids { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancel);
    }
}
