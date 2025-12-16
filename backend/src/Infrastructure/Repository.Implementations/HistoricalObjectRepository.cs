using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class HistoricalObjectRepository : IHistoricalObjectRepository
{
    private readonly MapDbContext _context;

    public HistoricalObjectRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(HistoricalObject histObject, CancellationToken ct)
    {
        await _context.HistoricalObjects.AddAsync(histObject, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<HistoricalObject?> GetByIdAsync(Guid histObjectId, CancellationToken ct)
    {
        return await _context.HistoricalObjects
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == histObjectId, ct);
    }

    public async Task<List<HistoricalObject>?> GetAllByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        return await _context.HistoricalObjects
            .AsNoTracking()
            .Where(o => o.LayerRegionId == layerRegionId)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(HistoricalObject histObject, CancellationToken ct)
    {
        var existing = await _context.HistoricalObjects
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == histObject.Id, ct);
        if (existing != null)
        {
            _context.HistoricalObjects.Update(histObject);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid histObjectId, CancellationToken ct)
    {
        var existing = await _context.HistoricalObjects
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == histObjectId, ct);
        if (existing != null)
        {
            _context.HistoricalObjects.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}