using System;
using System.Data.Entity;

namespace PriceSignageSystem.Models.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<STRPRC> STRPRC { get; set; }
        public DbSet<Type> Type { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}