using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class MapRepository : IMapRepository
{
    private readonly MapDbContext _context;

    public MapRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Map map, CancellationToken ct)
    {
        await _context.Maps.AddAsync(map, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Map?> GetByIdAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.Maps
            .Include(m => m.Regions)
                .ThenInclude(r => r.Region)
                    .ThenInclude(r => r.Geometry)
            .Include(m => m.HistoricalLine)
                .ThenInclude(l => l.HistoricalObjects)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == mapId, ct);
    }

    public async Task<Map?> GetByIdWithActiveRegionsAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.Maps
            .Include(m => m.Regions)
                .ThenInclude(r => r.Region)
                    .ThenInclude(r => r.Geometry)
            .Include(m => m.HistoricalLine)
                .ThenInclude(l => l.HistoricalObjects)
            .AsNoTracking()
            .Where(m => m.Regions.All(r => r.IsActive == true))
            .FirstOrDefaultAsync(m => m.Id == mapId, ct);
    }

    public async Task<List<Map>?> GetAllAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.Maps
            .Include(m => m.Regions)
                .ThenInclude(r => r.Region)
                    .ThenInclude(r => r.Geometry)
            .Include(m => m.HistoricalLine)
                .ThenInclude(l => l.HistoricalObjects)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Map map, CancellationToken ct)
    {
        var existing = await _context.Maps
            .FirstOrDefaultAsync(m => m.Id == map.Id, ct);
        if (existing != null)
        {
            _context.Maps.Update(map);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid mapId, CancellationToken ct)
    {
        var existing = await _context.Maps
            .FirstOrDefaultAsync(m => m.Id == mapId, ct);
        if (existing != null)
        {
            _context.Maps.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}