using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class LayerRegionRepository : ILayerRegionRepository
{
    private readonly MapDbContext _context;

    public LayerRegionRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(LayerRegion layerRegion, CancellationToken ct)
    {
        await _context.LayerRegions.AddAsync(layerRegion, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<LayerRegion?> GetNoActiveEmptyByNameAndMapIdAsync(string name, Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .Include(lr => lr.Region)
            .Include(lr => lr.Indicators)
            .Include(lr => lr.Style)
            .Include(lr => lr.HistoricalObjects)
            .Where(lr =>
                lr.MapId == mapId &&
                EF.Functions.ILike(lr.Region.Name, name) &&
                !lr.IsActive &&
                lr.Indicators == null &&
                lr.Style == null &&
                (lr.HistoricalObjects == null || !lr.HistoricalObjects.Any()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<LayerRegion?> GetHeaderByIdAsync(Guid regionId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .FirstOrDefaultAsync(lr => lr.Id == regionId, ct);
    }

    public async Task<LayerRegion?> GetWithRegionByIdAsync(Guid regionId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .Include(lr => lr.Region)
            .FirstOrDefaultAsync(lr => lr.Id == regionId, ct);
    }

    public async Task<List<Guid>> GetAllIdsByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .Where(lr => lr.MapId == mapId)
            .Select(lr => lr.Id)
            .ToListAsync(ct);
    }

    public async Task<List<LayerRegion>?> GetAllWithRegionAndGeometryByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .Include(lr => lr.Region)
                .ThenInclude(r => r.Geometry)
            .Where(lr => lr.MapId == mapId)
            .ToListAsync(ct);
    }

    public async Task<List<LayerRegion>?> GetAllActiveWithRegionAndGeometryByMapAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .AsNoTracking()
            .Include(lr => lr.Region)
                .ThenInclude(r => r.Geometry)
            .Where(lr => lr.MapId == mapId)
            .Where(lr => lr.IsActive)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(LayerRegion layerRegion, CancellationToken ct)
    {
        var existing = await _context.LayerRegions
            .FirstOrDefaultAsync(lr => lr.Id == layerRegion.Id, ct);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(layerRegion);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid regionId, CancellationToken ct)
    {
        var existing = await _context.LayerRegions
            .FirstOrDefaultAsync(lr => lr.Id == regionId, ct);
        if (existing != null)
        {
            _context.LayerRegions.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}