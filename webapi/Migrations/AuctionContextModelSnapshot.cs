﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using webapi.Database;

#nullable disable

namespace webapi.Migrations
{
    [DbContext(typeof(AuctionContext))]
    partial class AuctionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Auctions.Bid", b =>
                {
                    b.Property<long>("BidID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("BidID"));

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<int>("BidderId")
                        .HasColumnType("int");

                    b.Property<long>("ItemId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("BidID");

                    b.HasIndex("BidderId");

                    b.HasIndex("ItemId");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("Auctions.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Auctions.Item", b =>
                {
                    b.Property<long>("ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ItemID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<float?>("BuyPrice")
                        .HasColumnType("real");

                    b.Property<float>("Currently")
                        .HasColumnType("real");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Ends")
                        .HasColumnType("datetime2");

                    b.Property<float>("FirstBid")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumBids")
                        .HasColumnType("int");

                    b.Property<int>("SellerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("datetime2");

                    b.HasKey("ItemID");

                    b.HasIndex("SellerId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Auctions.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<DateTime?>("DateJoined")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuperuser")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CategoryItem", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<long>("ItemsItemID")
                        .HasColumnType("bigint");

                    b.HasKey("CategoriesId", "ItemsItemID");

                    b.HasIndex("ItemsItemID");

                    b.ToTable("ItemCategories", (string)null);
                });

            modelBuilder.Entity("Auctions.Bid", b =>
                {
                    b.HasOne("Auctions.User", "Bidder")
                        .WithMany("Bids")
                        .HasForeignKey("BidderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Auctions.Item", "Item")
                        .WithMany("Bids")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Bidder");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Auctions.Item", b =>
                {
                    b.HasOne("Auctions.User", "Seller")
                        .WithMany("Items")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("CategoryItem", b =>
                {
                    b.HasOne("Auctions.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Auctions.Item", null)
                        .WithMany()
                        .HasForeignKey("ItemsItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Auctions.Item", b =>
                {
                    b.Navigation("Bids");
                });

            modelBuilder.Entity("Auctions.User", b =>
                {
                    b.Navigation("Bids");

                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
