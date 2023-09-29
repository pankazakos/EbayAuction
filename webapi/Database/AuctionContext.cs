using Microsoft.EntityFrameworkCore; // DbContext, DbContextOptionsBuilder
using webapi.Models;

namespace webapi.Database
{

    public class AuctionContext : DbContext, IAuctionContext
    {
        public AuctionContext(DbContextOptions<AuctionContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Bid> Bids { get; set; }

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
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
