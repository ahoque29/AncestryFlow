using Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DatabaseContext : DbContext
{
    // Used by Ef Core to map the Member class to the Members table.
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DbSet<Member> Members { get; set; }

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Ancestry.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Father)
            .WithMany()
            .HasForeignKey(m => m.FatherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Mother)
            .WithMany()
            .HasForeignKey(m => m.MotherId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public async Task InitializeDatabase()
    {
        await Database.MigrateAsync();
    }
}