using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class RegionRepository : IRegionRepository 
{
    private readonly MapDbContext _context;

    public RegionRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Region region, CancellationToken ct)
    {
        await _context.Regions.AddAsync(region, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Region?> GetByIdAsync(Guid regionId, CancellationToken ct)
    {
        return await _context.Regions
            .AsNoTracking()
            .Include(r => r.Geometry)
            .FirstOrDefaultAsync(r => r.Id == regionId, ct);
    }

    public async Task<Region?> GetByNameAsync(string name, CancellationToken ct)
    {
        return await _context.Regions
            .AsNoTracking()
            .Include(r => r.Geometry)
            .FirstOrDefaultAsync(r => EF.Functions.ILike(r.Name, name), ct);
    }

    public async Task<List<Region>?> GetAllAsync(CancellationToken ct)
    {
        return await _context.Regions
            .AsNoTracking()
            .Include(r => r.Geometry)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Region region, CancellationToken ct)
    {
        var existingRegion = await _context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == region.Id, ct);
        if (existingRegion != null)
        {
            _context.Regions.Update(region);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid regionId, CancellationToken ct)
    {
        var existingRegion = await _context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == regionId, ct);
        if (existingRegion != null)
        {
            _context.Regions.Remove(existingRegion);
            await _context.SaveChangesAsync(ct);
        }
    }
}