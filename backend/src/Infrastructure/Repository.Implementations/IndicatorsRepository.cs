using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class IndicatorsRepository : IIndicatorsRepository
{
    private readonly MapDbContext _context;

    public IndicatorsRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(IndicatorsRegion indicators, CancellationToken ct)
    {
        await _context.IndicatorsRegions.AddAsync(indicators, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IndicatorsRegion?> GetByIdAsync(Guid indicatorsId, CancellationToken ct)
    {
        return await _context.IndicatorsRegions
            .FirstOrDefaultAsync(i => i.Id == indicatorsId, ct);
    }

    public async Task<IndicatorsRegion?> GetByLayerRegionAsync(Guid layerId, CancellationToken ct)
    {
        return await _context.IndicatorsRegions
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.RegionId == layerId, ct);
    }

    public async Task UpdateAsync(IndicatorsRegion indicators, CancellationToken ct)
    {
        var existing = await _context.IndicatorsRegions
            .FirstOrDefaultAsync(i => i.Id == indicators.Id, ct);
        if (existing != null)
        {
            _context.IndicatorsRegions.Update(existing);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid indicatorsId, CancellationToken ct)
    {
        var existing = await _context.IndicatorsRegions
            .FirstOrDefaultAsync(i => i.Id == indicatorsId, ct);
        if (existing != null)
        {
            _context.IndicatorsRegions.Update(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}