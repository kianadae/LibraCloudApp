using LibraCloud.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraCloud.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "C# Programming Basics", Author = "John Smith", QuantityAvailable = 5 },
                new Book { Id = 2, Title = "ASP.NET Core MVC Guide", Author = "Jane Doe", QuantityAvailable = 3 },
                new Book { Id = 3, Title = "Introduction to Databases", Author = "Alice Johnson", QuantityAvailable = 4 }
            );
        }
    }
}