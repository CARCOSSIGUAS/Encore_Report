using Belcorp.Encore.Entities.Entities.Core;
using Microsoft.EntityFrameworkCore;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreCore_Context : DbContext
    {
        public EncoreCore_Context(DbContextOptions<EncoreCore_Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Accounts>()
                .HasMany(p => p.AccountPhones);

            builder.Entity<Monitor>()
                .HasMany(p => p.MonitorDetails);

        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderCustomers> OrderCustomers { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<OrderItemPrices> OrderItemPrices { get; set; }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AccountPhones> AccountPhones { get; set; }

        public DbSet<Monitor> Monitor { get; set; }
        public DbSet<MonitorDetails> MonitorDetails { get; set; }

    }
}
