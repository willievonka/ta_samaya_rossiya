using Application.Queries;

namespace Application.Services.Interfaces;

public interface IQueryService
{
    Task<T[]> GetAsync<T>(DataQueryParams<T> queryParams, CancellationToken ct) where T : class;
    
    Task<int> GetCountAsync<T>(DataQueryParams<T> queryParams, CancellationToken ct) where T : class;
}