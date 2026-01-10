using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MapDbContext _context;

    public RefreshTokenRepository(MapDbContext mapDbContext)
    {
        _context = mapDbContext;
    }
    
    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> GetByTokenAndUserIdAsync(string refreshToken, Guid userId, CancellationToken ct)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId, ct);
    }
    
    public async Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == id, ct);
        if (existing != null)
        {
            _context.RefreshTokens.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteRangeAsync(List<RefreshToken> tokens, CancellationToken ct)
    {
        _context.RefreshTokens
            .RemoveRange(tokens);
        await _context.SaveChangesAsync(ct);
    }
}