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

    public async Task<Map?> GetHeaderByIdAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.Maps
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == mapId, ct);
    }

    public async Task<List<Map>?> GetAllHeadersAsync(CancellationToken ct)
    {
        return await _context.Maps
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Map map, CancellationToken ct)
    {
        var existing = await _context.Maps
            .AsNoTracking()
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