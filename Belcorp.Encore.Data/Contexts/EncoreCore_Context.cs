using Belcorp.Encore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
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
        }

        
    }
}
