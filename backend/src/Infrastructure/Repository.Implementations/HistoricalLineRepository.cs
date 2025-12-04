using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class HistoricalLineRepository : IHistoricalLineRepository
{
    private readonly MapDbContext _context;

    public HistoricalLineRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(HistoricalLine histLine, CancellationToken ct)
    {
        await _context.HistoricalLines.AddAsync(histLine, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<HistoricalLine?> GetHeaderByIdAsync(Guid histLineId, CancellationToken ct)
    {
        return await _context.HistoricalLines
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == histLineId, ct); 
    }


    public async Task<HistoricalLine?> GetByIdWithObjectsAsync(Guid histLineId, CancellationToken ct)
    {
        return await _context.HistoricalLines
            .Include(l => l.HistoricalObjects)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == histLineId, ct);
    }

    public async Task<HistoricalLine?> GetByMapWithObjectsAsync(Guid mapId, CancellationToken ct)
    {
        return await _context.HistoricalLines
            .Include(l => l.HistoricalObjects)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.MapId == mapId, ct);
    }

    public async Task UpdateAsync(HistoricalLine histLine, CancellationToken ct)
    {
        var existing = await _context.HistoricalLines
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == histLine.Id, ct);
        if (existing != null)
        {
            _context.HistoricalLines.Update(histLine);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByIdAsync(Guid histLineId, CancellationToken ct)
    {
        var existing = await _context.HistoricalLines
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == histLineId, ct);
        if (existing != null)
        {
            _context.HistoricalLines.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}