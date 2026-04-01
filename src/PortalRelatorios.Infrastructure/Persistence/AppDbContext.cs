using Microsoft.EntityFrameworkCore;
using PortalRelatorios.Domain.Entities;

namespace PortalRelatorios.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Username).HasMaxLength(256);
            e.Property(x => x.Name).HasMaxLength(512);
            e.Property(x => x.Email).HasMaxLength(512);
        });

        modelBuilder.Entity<Sector>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<Permission>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.SectorId }).IsUnique();
            e.HasOne(x => x.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Sector)
                .WithMany(s => s.Permissions)
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
