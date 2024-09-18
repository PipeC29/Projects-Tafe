using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Supermarket3.Models;

#nullable disable

namespace Supermarket3.Data
{
    public partial class Supermarket3DBContext : DbContext
    {
        public Supermarket3DBContext()
        {
        }

        public Supermarket3DBContext(DbContextOptions<Supermarket3DBContext> options)
            : base(options)
        {
        }

       
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users {get; set;}
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1"),
                    Role = "Admin"
                });

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Granny Smith Apples",
                    Unit = "1kg",
                    Price = 5.50m
                },
                new Product
                {
                    Id = 2,
                    Name = "Fresh tomatoes",
                    Unit = "500g",
                    Price = 5.90m
                },
                new Product
                {
                    Id = 3,
                    Name = "Watermelon",
                    Unit = "Whole",
                    Price = 6.60m
                });

            builder.Entity<ShoppingCart>().HasData(
                new ShoppingCart
                {
                    Id = 1,
                    UserId = 1,
                    Date = DateTime.Now,
                    Total = 32.98
                });

            builder.Entity<ShoppingCartItem>().HasData(
                new ShoppingCartItem
                {
                    Id = 1,
                    ShoppingCartId = 1,
                    ItemId = 1,
                    Quantity = 1
                },
                new ShoppingCartItem
                {
                    Id = 2,
                    ShoppingCartId = 1,
                    ItemId = 3,
                    Quantity = 1
                });

              builder.Entity<ShoppingCart>()
                .HasOne(sc => sc.CartUser)
                .WithMany(u => u.Carts) // Assuming the navigation property in User for ShoppingCart is named Carts
                .HasForeignKey(sc => sc.UserId);

            builder.Entity<ShoppingCartItem>()
                .HasOne(item => item.Cart)
                .WithMany(sc => sc.CartItems)
                .HasForeignKey(item => item.ShoppingCartId);

            builder.Entity<ShoppingCartItem>()
                .HasOne(item => item.ProductItem)
                .WithMany()
                .HasForeignKey(item => item.ItemId);

            builder.Entity<Product>(entity =>
            {
                //entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(builder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
