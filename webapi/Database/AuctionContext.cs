using Microsoft.EntityFrameworkCore; // DbContext, DbContextOptionsBuilder
using webapi.Models;

namespace webapi.Database
{

    public class AuctionContext : DbContext, IAuctionContext
    {
        public AuctionContext(DbContextOptions<AuctionContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = default!;

        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;

        public DbSet<Bid> Bids { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Item>()
                .HasMany(i => i.Categories)
                .WithMany(c => c.Items)
                .UsingEntity(j => j.ToTable("ItemCategories"));

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Seller)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Bids)
                .HasForeignKey(b => b.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancel = default)
        {
            var orphanUsers = ChangeTracker.Entries<User>()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity.Id)
                .ToList();

            foreach (var userId in orphanUsers)
            {
                var bidsToRemove = await Bids.Where(b => b.BidderId == userId).ToListAsync(cancel);
                Bids.RemoveRange(bidsToRemove);

                var itemsToRemove = await Items.Where(i => i.SellerId == userId).ToListAsync(cancel);
                Items.RemoveRange(itemsToRemove);
            }

            return await base.SaveChangesAsync(cancel);
        }

    }
}
