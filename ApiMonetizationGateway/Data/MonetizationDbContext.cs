using ApiMonetizationGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ApiMonetizationGateway.Data
{
    public class MonetizationDbContext : DbContext
    {
        public MonetizationDbContext(DbContextOptions<MonetizationDbContext> options)
            : base(options) { }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ApiEndpoint> ApiEndpoints { get; set; }
        public DbSet<UsageLog> UsageLogs { get; set; }
        public DbSet<MonthlySummary> MonthlySummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Subscription)
                .WithOne()
                .HasForeignKey<Subscription>(s => s.CustomerId);

            modelBuilder.Entity<UsageLog>()
                .HasOne(l => l.ApiEndpoint)
                .WithMany()
                .HasForeignKey(l => l.ApiId);

            //Data Seeding:
            modelBuilder.Entity<Tier>().HasData(
                new Tier
                {
                    TierId = 1,
                    Name = "Free",
                    MonthlyQuota = 100,
                    RateLimit = 2,
                    Price = 0
                },
                new Tier
                {
                    TierId = 2,
                    Name = "Pro",
                    MonthlyQuota = 100000,
                    RateLimit = 10,
                    Price = 50
                }
            );

            modelBuilder.Entity<ApiEndpoint>().HasData(
                new ApiEndpoint
                {
                    ApiId = 1,
                    Name = "Data API",
                    Path = "/api/data",
                    IsActive = true
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, Name = "Test", ApiKey = "test-key-123" }
            );

            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    SubscriptionId = 1,
                    CustomerId = 1,
                    TierId = 1,
                    StartDate = DateTime.UtcNow
                }
            );
        }
    }
}