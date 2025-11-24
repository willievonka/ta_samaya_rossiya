using Application.Queries;
using Application.Services.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Services.Implementations;

/// <summary>
/// Сервис выполнения параметризированных запросов к бд
/// </summary>
public class QueryService : IQueryService
{
    private readonly MapDbContext  _dbContext;

    public QueryService(MapDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Выполнить запрос на получение объектов к базе данных с указанным параметрами запроса.
    /// </summary>
    /// <returns>
    /// Возвращается массив объектов, которые соответствуют запросу.
    /// </returns>
    public async Task<T[]> GetAsync<T>(DataQueryParams<T> queryParams, CancellationToken ct) where T : class
    {
        var set = _dbContext.Set<T>().AsQueryable();
        if (queryParams.Expression != null)
        {
            set = set.Where(queryParams.Expression);
        }
        
        if (queryParams.Filters != null)
        {
            set = queryParams.Filters.Aggregate(set, (current, filter) => current.Where(filter));
        }
        
        if (queryParams.Sorting != null)
        {
            if (queryParams.Sorting.OrderBy == null)
            {
                if (queryParams.Sorting.PropertyName != null)
                {
                    set = queryParams.Sorting.Ascending ? set.OrderBy(queryParams.Sorting.PropertyName) : 
                        set.OrderBy(queryParams.Sorting.PropertyName + " descending");
                }
            }
            else
            {
                if (queryParams.Sorting.ThenBy != null)
                {
                    set = queryParams.Sorting.Ascending ? 
                        set.OrderBy(queryParams.Sorting.OrderBy).ThenBy(queryParams.Sorting.ThenBy) : 
                        set.OrderByDescending(queryParams.Sorting.OrderBy).ThenByDescending(queryParams.Sorting.ThenBy);
                }
                else
                {
                    set = queryParams.Sorting.Ascending ? 
                        set.OrderBy(queryParams.Sorting.OrderBy) : 
                        set.OrderByDescending(queryParams.Sorting.OrderBy);
                }
            }
        }
        
        if (queryParams.Paging != null)
        {
            set = set.Skip(queryParams.Paging.Skip).Take(queryParams.Paging.Take);
        }
        
        set = ApplyIncludeParams(set, queryParams.IncludeParams);
        
        return await set.ToArrayAsync(ct);
    }

    /// <summary>
    /// Выполнить запрос на получение кол-ва объектов в базе данных с указанным параметрами запроса. Параметры Paging, Order и Include игнорируются.
    /// </summary>
    /// <returns>
    /// Возвращается кол-во объектов, которые соответствуют запросу.
    /// </returns>
    public async Task<int> GetCountAsync<T>(DataQueryParams<T> queryParams, CancellationToken ct) where T : class
    {
        var set = _dbContext.Set<T>().AsQueryable();
        if (queryParams.Expression != null)
        {
            set = set.Where(queryParams.Expression);
        }

        if (queryParams.Filters != null)
        {
            foreach (var filter in queryParams.Filters)
            {
                set = set.Where(filter);
            }
        }
        
        return await set.CountAsync(ct);
    }
    
    /// <summary>
    /// Включение других таблиц
    /// </summary>
    /// <param name="set">query set</param>
    /// <param name="includeParams">Параметры для включения</param>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <returns>query set</returns>
    private IQueryable<T> ApplyIncludeParams<T>(IQueryable<T> set, IncludeParams<T>? includeParams) where T : class
    {
        if (includeParams?.IncludeProperties?.Count > 0)
            foreach (var includeProp in includeParams.IncludeProperties)
                set = set.Include(includeProp);
        
        return set;
    }
}