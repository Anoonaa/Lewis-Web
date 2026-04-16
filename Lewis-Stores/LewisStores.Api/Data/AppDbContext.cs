using Microsoft.EntityFrameworkCore;
using LewisStores.Api.Models;

namespace LewisStores.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<CreditApplication> CreditApplications { get; set; } = null!;
        public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>().HasKey(c => c.InternalId);
            modelBuilder.Entity<CreditApplication>().HasKey(c => c.Id);
            modelBuilder.Entity<PaymentMethod>().HasKey(p => p.Id);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "cat-1", Name = "Furniture", Description = "Architectural sofas, dining, and lounge essentials", To = "/products", Tone = "category-furniture" },
                new Category { Id = "cat-2", Name = "Appliances", Description = "Performance-first pieces for modern homes", To = "/products", Tone = "category-appliances" },
                new Category { Id = "cat-3", Name = "Electronics", Description = "Curated home tech and sound systems", To = "/products", Tone = "category-electronics" },
                new Category { Id = "cat-4", Name = "Decor", Description = "Lighting, art, and finishing details", To = "/products", Tone = "category-decor" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = "luca-modular", Title = "Luca Modular Sofa", Description = "Textured ivory upholstery with brushed oak legs.", Price = 24999, OldPrice = 27999, Tag = "Limited Edition", Rating = 4.8 },
                new Product { Id = "atlas-lounge", Title = "Atlas Lounge Chair", Description = "Low-profile silhouette with layered cushioning.", Price = 10999, OldPrice = null, Tag = null, Rating = 4.6 },
                new Product { Id = "miren-table", Title = "Miren Coffee Table", Description = "Solid ash base and honed stone top.", Price = 7699, OldPrice = null, Tag = null, Rating = 4.7 },
                new Product { Id = "solvi-console", Title = "Solvi Console", Description = "Storage-forward entry piece with hidden cable tray.", Price = 8999, OldPrice = null, Tag = null, Rating = 4.5 }
            );

            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { InternalId = 1, Id = "luca-modular", Title = "Luca Modular Sofa", Variant = "Pearl Cloud, Matte Black Legs", Quantity = 1, Price = 24999 },
                new CartItem { InternalId = 2, Id = "miren-table", Title = "Miren Coffee Table", Variant = "Ash + Stone", Quantity = 1, Price = 7699 }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = "LWS-20419", Date = "08 Apr 2026", Status = "Delivered", Total = 11799, UserId = "user-1", Items = "Samsung 65\" 4K Smart TV + LG Soundbar" },
                new Order { Id = "LWS-20388", Date = "01 Apr 2026", Status = "Shipped", Total = 24999, UserId = "user-1", Items = "Luca Modular Sofa + Miren Coffee Table" },
                new Order { Id = "LWS-20293", Date = "22 Mar 2026", Status = "Processing", Total = 7699, UserId = "user-1", Items = "Defy 8kg Front Loader Washing Machine" },
                new Order { Id = "LWS-20144", Date = "13 Mar 2026", Status = "Delivered", Total = 19999, UserId = "user-1", Items = "Samsung 580L Double Door Fridge" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = "user-1", Email = "test@student.com", Password = "password", Role = "Customer", FullName = "Test Customer", Phone = "+27 82 555 0123", Address = "12 Mandela Street, Sandton, Johannesburg, 2196" },
                new User { Id = "admin-1", Email = "admin@lewis.com", Password = "password", Role = "Admin", FullName = "Lewis Admin", Phone = "+27 82 000 1111", Address = "Head Office" }
            );

            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = 1, UserId = "user-1", CardholderName = "Test Customer", Last4 = "4242", Brand = "Visa", Expiry = "12/29", IsDefault = true }
            );
        }
    }
}
