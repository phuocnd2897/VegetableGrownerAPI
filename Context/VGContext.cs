
using Microsoft.EntityFrameworkCore;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Context
{
    public class VGContext : DbContext
    {
        public VGContext(DbContextOptions<VGContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
        public virtual DbSet<Account> AppUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
