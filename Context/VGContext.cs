
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
        public virtual DbSet<AppAccount> Accounts { get; set; }
        public virtual DbSet<AccountRequest> AccountRequests { get; set; }
        public virtual DbSet<AccountFriend> AccountFriends { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<ShareDetail> ShareDetails { get; set; }
        public virtual DbSet<ExchangeDetail> ExchangeDetails { get; set; }
        public virtual DbSet<VegetableImage> VegetableImages { get; set; }
        public virtual DbSet<AccountRole> Roles { get; set; }
        public virtual DbSet<Garden> Gardens { get; set; }
        public virtual DbSet<Vegetable> Vegetables { get; set; }
        public virtual DbSet<VegetableDescription> VegetableDescriptions { get; set; }
        public virtual DbSet<VegetableComposition> VegetableCompositions { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<PrecentReport> PrecentReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
