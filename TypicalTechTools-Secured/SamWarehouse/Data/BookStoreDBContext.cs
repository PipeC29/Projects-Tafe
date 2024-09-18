using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BookStoreApp.Models;

#nullable disable

namespace BookStoreApp.Data
{
    public partial class BookStoreDBContext : DbContext
    {
        public BookStoreDBContext()
        {
        }

        public BookStoreDBContext(DbContextOptions<BookStoreDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
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

            builder.Entity<Author>().HasData(
                new Author 
                {
                    Id = 1,
                    LastName = "King",
                    FirstName = "Stephen"
                },
                new Author 
                {
                    Id = 2,
                    LastName = "Hill",
                    FirstName = "Joe"
                },
                new Author
                {
                    Id = 3,
                    LastName = "Atwood",
                    FirstName = "Margaret"
                });

            builder.Entity<Book>().HasData(
                new Book 
                {
                    Id = 1,
                    AuthorId= 1,
                    Name= "Granny Smith Apples",
                    Unit= "1kg",
                    Price= 5.50m
                },
                new Book 
                {
                    Id = 2,
                    AuthorId= 2,
                    Name = "Fresh tomatoes",
                    Unit = "500g",
                    Price = 5.90m
                },
                new Book 
                {
                    Id = 3,
                    AuthorId= 3,
                    Name= "Watermelon",
                    Unit = "Whole",
                    Price = 6.60m
                });

            builder.Entity<ShoppingCart>().HasData(
                new ShoppingCart 
                {
                    Id = 1,
                    UserId= 1,
                    Date= DateTime.Now,
                    Total = 32.98
                });

            builder.Entity<ShoppingCartItem>().HasData(
                new ShoppingCartItem 
                {
                    Id = 1,
                    ShoppingCartId= 1,
                    BookId= 1,
                    Quantity= 1
                },
                new ShoppingCartItem 
                {
                    Id = 2,
                    ShoppingCartId= 1,
                    BookId= 3,
                    Quantity= 1
                });


            builder.Entity<Author>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            builder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Books_Authors");
            });

            OnModelCreatingPartial(builder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
