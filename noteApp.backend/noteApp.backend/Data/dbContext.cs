using Microsoft.EntityFrameworkCore;
using noteApp.backend.Models;
using System.Security.Claims;

namespace noteApp.backend.Data
{
    public class dbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }

        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => 
            { 
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<User>()
                .Property(b => b.Id)
                .HasDefaultValueSql("NEWID()");
        }

        public override int SaveChanges()
        {
            AddTimestamps(); 
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    if (entity.Entity is Note note)
                    {
                        note.CreatedDate = DateTime.UtcNow;
                    }
                }

                if (entity.Entity is Note noteModified)
                {
                    noteModified.ModifiedDate = DateTime.UtcNow;
                }
            }
        }
    }
}
