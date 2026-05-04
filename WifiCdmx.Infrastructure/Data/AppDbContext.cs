using Microsoft.EntityFrameworkCore;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WifiPoint> WifiPoints => Set<WifiPoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WifiPoint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Neighborhood).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Borough).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Program).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.AccessPointCount).IsRequired();

            entity.HasIndex(e => e.Borough);
            entity.HasIndex(e => e.Program);
        });
    }
}
