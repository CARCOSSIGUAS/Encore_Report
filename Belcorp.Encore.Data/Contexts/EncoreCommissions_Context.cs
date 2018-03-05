using Belcorp.Encore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreCommissions_Context : DbContext
    {
        public EncoreCommissions_Context(DbContextOptions<EncoreCommissions_Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }

        public DbSet<AccountsInformation> AccountInformation { get; set; }

    }
}
