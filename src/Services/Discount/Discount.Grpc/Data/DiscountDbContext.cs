using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>().HasData(
            new Coupon
            {
                Id = 1,
                ProductName = "iPhone 14",
                Description = "iPhone 14 discount",
                Amount = 15,
            },
             new Coupon
             {
                 Id = 2,
                 ProductName = "Samsung Galaxy S23 Ultra",
                 Description = "Samsung Galaxy S23 Ultra discount",
                 Amount = 20,
             }
        );
    }
}
