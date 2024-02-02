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
        public DbSet<Country> Countries { get; set; }
        public DbSet<ItemQueue> ItemQueues { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<STRPRCLog> STRPRCLogs { get; set; }
        public DbSet<EditReason> EditReasons { get; set; }
        public DbSet<InventoryPrintingLog> InventoryPrintingLogs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}