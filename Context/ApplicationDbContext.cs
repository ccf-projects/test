using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using BookWeb.Models;

namespace BookWeb.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map the JsonData property to the jsonb column
            modelBuilder.Entity<Transaction>()
                .Property(e => e.JsonData)
                .HasColumnType("jsonb");

            base.OnModelCreating(modelBuilder);
        }
    }
}