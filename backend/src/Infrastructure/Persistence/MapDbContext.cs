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
    public DbSet<HistoricalObject> HistoricalObjects => Set<HistoricalObject>();
    public DbSet<IndicatorsRegion> IndicatorsRegions => Set<IndicatorsRegion>();
    public DbSet<LayerRegion> LayerRegions => Set<LayerRegion>();
    public DbSet<RegionGeometry> RegionGeometries => Set<RegionGeometry>();
    public DbSet<LayerRegionStyle> LayerRegionStyles => Set<LayerRegionStyle>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>()
            .HasMany(m => m.Regions)
            .WithOne(r => r.Map)
            .HasForeignKey(r => r.MapId)
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
            .HasOne(ho => ho.LayerRegion)
            .WithMany(hl => hl.HistoricalObjects)
            .HasForeignKey(ho => ho.LayerRegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<RegionGeometry>()
            .HasOne(rg => rg.Region)
            .WithOne(r => r.Geometry)
            .HasForeignKey<RegionGeometry>(rg => rg.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<LayerRegionStyle>()
            .HasOne(s => s.Region)
            .WithOne(r => r.Style)
            .HasForeignKey<LayerRegionStyle>(s => s.RegionId)
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
        
        modelBuilder.Entity<HistoricalObject>()
            .HasKey(ho => ho.Id);
        
        modelBuilder.Entity<IndicatorsRegion>()
            .HasKey(ir => ir.Id);
        
        modelBuilder.Entity<RegionGeometry>()
            .HasKey(rg => rg.Id);
        
        modelBuilder.Entity<LayerRegion>()
            .HasKey(rl => rl.Id);
        
        modelBuilder.Entity<LayerRegionStyle>()
            .HasKey(rls => rls.Id);
        
        modelBuilder.Entity<Admin>()
            .HasKey(a => a.Id);
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
        
        modelBuilder.Entity<HistoricalObject>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<IndicatorsRegion>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<LayerRegion>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<LayerRegionStyle>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Admin>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }
}