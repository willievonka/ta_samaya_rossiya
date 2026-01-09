using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByTokenAndUserIdAsync(string refreshToken, Guid userId, CancellationToken ct);
    Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
    Task DeleteByIdAsync(Guid id, CancellationToken ct);
    Task DeleteRangeAsync(List<RefreshToken> tokens, CancellationToken ct);
}