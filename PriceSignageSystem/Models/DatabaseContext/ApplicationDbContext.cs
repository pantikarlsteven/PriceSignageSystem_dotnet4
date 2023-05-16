using System;
using System.Data.Entity;

namespace PriceSignageSystem.Models.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=MyConnectionString")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<STRPRC> STRPRCs { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}