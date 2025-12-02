using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class MapDbContext : DbContext
{
    public MapDbContext(DbContextOptions<MapDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Map> Maps => Set<Map>();
    public DbSet<HistoricalLine> HistoricalLines => Set<HistoricalLine>();
    public DbSet<HistoricalObject> HistoricalObjects => Set<HistoricalObject>();
    public DbSet<IndicatorsRegion> IndicatorsRegions => Set<IndicatorsRegion>();
    public DbSet<LayerRegion> LayerRegions => Set<LayerRegion>();
    public DbSet<RegionGeometry> RegionGeometries => Set<RegionGeometry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>()
            .HasMany(m => m.Regions)
            .WithOne(r => r.Map)
            .HasForeignKey(r => r.MapId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HistoricalLine>()
            .HasOne(l => l.Map)
            .WithOne(m => m.HistoricalLine)
            .HasForeignKey<HistoricalLine>(l => l.MapId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<LayerRegion>()
            .HasOne(lr => lr.Region)
            .WithMany()
            .HasForeignKey(lr => lr.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<IndicatorsRegion>()
            .HasOne(ir => ir.Region)
            .WithOne(lr => lr.Indicators)
            .HasForeignKey<IndicatorsRegion>(ir => ir.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<HistoricalObject>()
            .HasOne(ho => ho.HistoricalLine)
            .WithMany(hl => hl.HistoricalObjects)
            .HasForeignKey(ho => ho.LineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<RegionGeometry>()
            .HasOne(rg => rg.Region)
            .WithOne(r => r.Geometry)
            .HasForeignKey<RegionGeometry>(rg => rg.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Map>()
            .Property(m => m.CreatedAt)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        
        modelBuilder.Entity<Map>()
            .Property(m => m.UpdatedAt)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        
        AddPrimaryKey(modelBuilder);
        
        AddAutoGeneratingId(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
    
    private void AddPrimaryKey(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Region>()
            .HasKey(r => r.Id);
        
        modelBuilder.Entity<Map>()
            .HasKey(m => m.Id);
        
        modelBuilder.Entity<HistoricalLine>()
            .HasKey(hl => hl.Id);
        
        modelBuilder.Entity<HistoricalObject>()
            .HasKey(ho => ho.Id);
        
        modelBuilder.Entity<IndicatorsRegion>()
            .HasKey(ir => ir.Id);
        
        modelBuilder.Entity<RegionGeometry>()
            .HasKey(rg => rg.Id);
        
        modelBuilder.Entity<LayerRegion>()
            .HasKey(rl => rl.Id);
    }
    
    private void AddAutoGeneratingId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Region>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<RegionGeometry>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Map>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<HistoricalLine>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<HistoricalObject>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<IndicatorsRegion>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<LayerRegion>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }
}