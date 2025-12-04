using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class LayerRegionStyleRepository : ILayerRegionStyleRepository
{
    private readonly MapDbContext _context;

    public LayerRegionStyleRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(LayerRegionStyle style, CancellationToken ct)
    {
        await _context.LayerRegionStyles.AddAsync(style, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<LayerRegionStyle?> GetByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        return await _context.LayerRegionStyles
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.RegionId == layerRegionId, ct);
    }

    public async Task UpdateAsync(LayerRegionStyle style, CancellationToken ct)
    {
        var existing = await _context.LayerRegionStyles
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == style.Id, ct);
        if (existing != null)
        {
            _context.LayerRegionStyles.Update(style);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        var existing = await _context.LayerRegionStyles
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.RegionId == layerRegionId, ct);
        if (existing != null)
        {
            _context.LayerRegionStyles.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}