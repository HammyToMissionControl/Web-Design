using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole { Id="2", Name = "Admin", NormalizedName = "ADMIN"});

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fruit" },
                new Category { Id = 2, Name = "Vegetable" },
                new Category { Id = 3, Name = "Dairy"},
                new Category { Id = 4, Name = "Meat"});
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Basket> baskets { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<BasketItem> items { get; set; }
    }
}
