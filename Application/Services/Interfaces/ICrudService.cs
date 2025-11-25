using Application.Queries;

namespace Application.Services.Interfaces;

public interface ICrudService
{
    Task<T?> GetByIdOrDefaultAsync<T>(Guid id, IncludeParams<T> includeParams, CancellationToken ct) where T : class;
    Task<T?> GetByIdOrDefaultAsync<T>(Guid id, CancellationToken ct) where T : class;
    Task<bool> TryRemoveAsync<T>(Guid id, CancellationToken ct) where T : class;
    Task RemoveRangeAsync<T>(CancellationToken ct, params Guid[] ids) where T : class;
    Task RemoveRangeAsync<T>(CancellationToken ct, params T[] entities)  where T : class;
    Task<Guid> CreateAsync<T>(T entity, CancellationToken ct)  where T : class;
    Task<Guid> UpdateAsync<T>(T entity, CancellationToken ct) where T : class;
}