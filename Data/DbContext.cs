using MemoryboardAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemoryboardAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Clipboard> Clipboards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Clipboard)
                .WithOne(c => c.User)
                .HasForeignKey<Clipboard>(c => c.UserId)
                .IsRequired();
        }
    }
}