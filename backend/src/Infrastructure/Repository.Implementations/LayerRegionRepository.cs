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

    public async Task<LayerRegion?> GetByIdAsync(Guid regionId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .Include(lr => lr.Region)
                .ThenInclude(r => r.Geometry)
            .Include(lr => lr.Indicators)
            .AsNoTracking()
            .FirstOrDefaultAsync(lr => lr.Id == regionId, ct);
    }

    public async Task<List<LayerRegion>?> GetAllByMapAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .Include(lr => lr.Region)
                .ThenInclude(r => r.Geometry)
            .Include(lr => lr.Indicators)
            .Where(lr => lr.MapId == mapId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<LayerRegion>?> GetAllActiveByMapAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.LayerRegions
            .Include(lr => lr.Region)
                .ThenInclude(r => r.Geometry)
            .Include(lr => lr.Indicators)
            .Where(lr => lr.MapId == mapId)
            .Where(lr => lr.IsActive)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(LayerRegion layerRegion, CancellationToken ct)
    {
        var existing = await _context.LayerRegions
            .FirstOrDefaultAsync(lr => lr.Id == layerRegion.Id, ct);
        if (existing != null)
        {
            _context.LayerRegions.Update(layerRegion);
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