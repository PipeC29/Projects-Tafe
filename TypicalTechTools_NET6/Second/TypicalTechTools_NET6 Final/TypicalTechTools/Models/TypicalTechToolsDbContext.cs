using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace TypicalTechTools.Models
{
    public class TypicalTechToolsDbContext: DbContext
    {
        public TypicalTechToolsDbContext(DbContextOptions options): base(options) 
        {

        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Product>().HasData(
                new Product 
                { ProductCode = 12345,
                    ProductName = "Generic Headphones", 
                    ProductPrice = (decimal)84.99,
                    ProductDescription = "bluetooth headphones with fair battery life and a 1 month warranty",
                    UpdatedDate = new DateTime(2023, 12, 5) 
                });

            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                 CommentId = 1,
                 CommentText =   "Value for money",
                 SessionId = null,
                 ProductCode = 12345,
                 CreatedDate = new DateTime(2023 ,7,27)
                              
                });

            // Only to build DB,remove once database is running
                modelBuilder.Entity<AdminUser>().HasData(
                    new AdminUser
                {
                    Id = 1,
                    UserName = "Admin",
                    
                    PasswordHarsh = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_11"),
                    IsAdmin = true,
                });
        }
       
    }
}
