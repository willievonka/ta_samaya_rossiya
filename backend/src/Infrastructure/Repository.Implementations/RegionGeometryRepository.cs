using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class RegionGeometryRepository : IRegionGeometryRepository
{
    private readonly MapDbContext _context;

    public RegionGeometryRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(RegionGeometry regionGeometry, CancellationToken ct)
    {
        await _context.AddAsync(regionGeometry, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<RegionGeometry?> GetByIdAsync(Guid geometryId, CancellationToken ct)
    {
        return await _context.RegionGeometries
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == geometryId, ct);
    }

    public Task<RegionGeometry?> GetByRegionAsync(Guid regionId, CancellationToken ct)
    {
        return _context.RegionGeometries
            .Where(g => g.RegionId == regionId)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateAsync(RegionGeometry regionGeometry, CancellationToken ct)
    {
        var existingRegionGeometry = await _context.RegionGeometries
            .FirstOrDefaultAsync(g => g.Id == regionGeometry.Id, ct);
        if (existingRegionGeometry != null)
        {
            _context.RegionGeometries.Update(existingRegionGeometry);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid geometryId, CancellationToken ct)
    {
        var geometry = await _context.RegionGeometries
            .FirstOrDefaultAsync(g => g.Id == geometryId, ct);
        if (geometry != null)
        {
            _context.RegionGeometries.Remove(geometry);
            await _context.SaveChangesAsync(ct);
        }
    }
}