using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreCommissions_Context : DbContext
    {
        public EncoreCommissions_Context(DbContextOptions<EncoreCommissions_Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AccountKPIs>()
                .HasKey(c => new { c.AccountID, c.PeriodID, c.CalculationTypeID });

            builder.Entity<OrderCalculationsOnline>()
                .HasKey(c => new { c.AccountID, c.OrderID, c.OrderCalculationTypeID });

        }

        public DbSet<AccountsInformation> AccountsInformation { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Titles> Titles { get; set; }
		public DbSet<SponsorTree> SponsorTree { get; set; }
        public DbSet<CalculationTypes> CalculationTypes { get; set; }
		public DbSet<AccountKPIs> AccountKPIs { get; set; }
        public DbSet<OrderCalculationsOnline> OrderCalculationsOnline { get; set; }

        public DbSet<OrderCalculationTypes> OrderCalculationTypes { get; set; }


    }
}
