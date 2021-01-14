
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
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountDetail> AccountDetails { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostImage> PostImages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
