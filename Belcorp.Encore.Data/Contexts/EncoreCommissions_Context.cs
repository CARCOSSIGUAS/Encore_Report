﻿using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities.Entities.Commissions;

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

            builder.Entity<RuleTypes>()
                .HasOne(p => p.RequirementRules);

            builder.Entity<RequirementRules>()
                .HasKey(c => new { c.RuleRequirementID, c.RuleTypeID });

            builder.Entity<PersonalIndicatorLog>()
                .HasMany(p => p.PersonalIndicatorDetailLog);

        }

        public DbSet<AccountsInformation> AccountsInformation { get; set; }
        public DbSet<Accounts> Accounts { get; set; }

        public DbSet<Titles> Titles { get; set; }
		public DbSet<SponsorTree> SponsorTree { get; set; }
        public DbSet<CalculationTypes> CalculationTypes { get; set; }
		public DbSet<AccountKPIs> AccountKPIs { get; set; }
        public DbSet<OrderCalculationsOnline> OrderCalculationsOnline { get; set; }

        public DbSet<OrderCalculationTypes> OrderCalculationTypes { get; set; }

        public DbSet<RuleTypes> RuleTypes { get; set; }
        public DbSet<RequirementRules> RequirementRules { get; set; }

        public DbSet<Periods> Periods { get; set; }
        public DbSet<AccountKPIsDetails> AccountKPIsDetails { get; set; }
        public DbSet<BonusDetails> BonusDetails { get; set; }

        public DbSet<PersonalIndicatorLog> PersonalIndicatorLog { get; set; }
        public DbSet<PersonalIndicatorDetailLog> PersonalIndicatorDetailLog { get; set; }
    }
}
