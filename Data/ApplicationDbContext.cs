using Microsoft.EntityFrameworkCore;
using SmartBuildingIoT.Models;

namespace SmartBuildingIoT.Data;

/// <summary>
/// Entity Framework Core database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SensorData> SensorReadings { get; set; }
    public DbSet<BuildingZone> Zones { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial zones
        modelBuilder.Entity<BuildingZone>().HasData(
            new BuildingZone { Id = 1, Name = "Lobby", Floor = 1, Type = "Lobby" },
            new BuildingZone { Id = 2, Name = "Office A", Floor = 2, Type = "Office" },
            new BuildingZone { Id = 3, Name = "Office B", Floor = 2, Type = "Office" },
            new BuildingZone { Id = 4, Name = "Meeting Room 1", Floor = 3, Type = "Meeting" },
            new BuildingZone { Id = 5, Name = "Server Room", Floor = 1, Type = "Technical" }
        );

        // Index for faster queries
        modelBuilder.Entity<SensorData>()
            .HasIndex(s => new { s.SensorId, s.Timestamp });
    }
}
