using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Data
{
    public class SportsDbContext : DbContext
    {
        public DbSet<Fan> Fans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SportClub> SportClubs { get; set; }

        public SportsDbContext(DbContextOptions<SportsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table names to be singular
            modelBuilder.Entity<Fan>().ToTable("Fan");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<SportClub>().ToTable("SportClub");

            // Configure composite key for Subscription
            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.FanId, s.SportClubId });

            // Example configuration: make sure LastName and FirstName are required
            modelBuilder.Entity<Fan>()
                .Property(f => f.LastName)
                .IsRequired();

            modelBuilder.Entity<Fan>()
                .Property(f => f.FirstName)
                .IsRequired();

            // Example configuration: set maximum length for Title
            modelBuilder.Entity<SportClub>()
                .Property(sc => sc.Title)
                .HasMaxLength(50)
                .IsRequired();

            // Example configuration: set DataType.Currency and ColumnType money for Fee
            modelBuilder.Entity<SportClub>()
                .Property(sc => sc.Fee)
                .HasColumnType("money")
                .IsRequired();
        }
    }

}
