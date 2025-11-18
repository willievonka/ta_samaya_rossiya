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
    public DbSet<LineRegion> LineRegions => Set<LineRegion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>()
            .HasMany(m => m.Regions)
            .WithMany(r => r.Maps);
        
        modelBuilder.Entity<HistoricalObject>()
            .HasOne(ho => ho.HistoricalLine)
            .WithMany()
            .HasForeignKey(ho => ho.LineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<IndicatorsRegion>()
            .HasOne(ir => ir.Region)
            .WithMany()
            .HasForeignKey(ir => ir.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<LineRegion>()
            .HasOne(lr => lr.Region)
            .WithMany()
            .HasForeignKey(lr => lr.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<LineRegion>()
            .HasOne(lr => lr.Line)
            .WithMany()
            .HasForeignKey(lr => lr.LineId)
            .OnDelete(DeleteBehavior.Cascade);

        AddPrimaryKey(modelBuilder);
        
        AddAutoGeneratingId(modelBuilder);
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
        
        modelBuilder.Entity<LineRegion>()
            .HasKey(lr => lr.Id);
    }
    
    private void AddAutoGeneratingId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Region>()
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
        
        modelBuilder.Entity<LineRegion>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }
}