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

            builder.Entity<Accounts>()
                .HasMany(p => p.AccountAddresses);

            builder.Entity<AccountAddresses>()
                .HasOne(p => p.Addresses);

            builder.Entity<AccountAddresses>()
                .HasKey(p => new { p.AccountID, p.AddressID });

            builder.Entity<Monitor>()
                .HasMany(p => p.MonitorDetails);

            builder.Entity<MonitorLotes>()
                .HasMany(p => p.MonitorOrders);

            builder.Entity<MonitorLotes>()
                .HasMany(p => p.MonitorOrders);

            builder.Entity<Activities>()
               .HasOne(p => p.ActivityStatuses);

            builder.Entity<TermTranslationsMongo>()
                .HasOne(p => p.Languages);

        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderCustomers> OrderCustomers { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<OrderItemPrices> OrderItemPrices { get; set; }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AccountPhones> AccountPhones { get; set; }
        

        public DbSet<Monitor> Monitor { get; set; }
        public DbSet<MonitorDetails> MonitorDetails { get; set; }

        public DbSet<MonitorLotes> MonitorLotes { get; set; }
        public DbSet<MonitorOrders> MonitorOrders { get; set; }


        public DbSet<Activities> Activities { get; set; }
        public DbSet<ActivityStatuses> ActivityStatuses { get; set; }


        public DbSet<Languages> Languages { get; set; }
        public DbSet<TermTranslationsMongo> TermTranslationsMongo { get; set; }

    }
}
