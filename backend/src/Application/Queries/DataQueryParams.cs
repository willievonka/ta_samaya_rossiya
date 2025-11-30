using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace Application.Queries;

/// <summary>
/// Параметры для выполнения запросов к бд
/// </summary>
/// <typeparam name="TEntity">Сущность из бд</typeparam>
public class DataQueryParams<TEntity> where TEntity : class
{
    public Expression<Func<TEntity, bool>>? Expression { get; set; }
    
    public PagingParams? Paging { get; set; }
    
    public SortingParams<TEntity>? Sorting { get; set; }
    
    public List<Expression<Func<TEntity, bool>>>? Filters { get; set; }
    
    public IncludeParams<TEntity>? IncludeParams { get; set; }
    
    public IQueryable<TEntity> Accumulate(IQueryable<TEntity> set)
    {
        set = ApplyFilters(set);
        set = ApplySorting(set);
        
            
        return set;
    }

    private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> set)
    {
        if  (Sorting == null)
            return set;

        if (Sorting.OrderBy != null)
        {
            var ordered = Sorting.Ascending 
                ? set.OrderBy(Sorting.OrderBy)
                : set.OrderByDescending(Sorting.OrderBy);

            if (Sorting.ThenBy != null)
            {
                ordered = Sorting.Ascending
                    ? ordered.ThenBy(Sorting.OrderBy)
                    : ordered.ThenByDescending(Sorting.OrderBy);;
            }
            
            set =  ordered;
        }
        
        else if (Sorting.PropertyName != null)
        {
            var ordered = Sorting.Ascending 
                ? set.OrderBy(Sorting.PropertyName)
                : set.OrderBy(Sorting.PropertyName +  " descending");
        }

        return set;
    }

    private IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> set)
    {
        if (Paging != null)
        {
            set = set.Skip(Paging.Skip).Take(Paging.Take);
        }
        
        return set;
    }

    private IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> enumerable)
    {
        if (Filters == null)
        {
            return enumerable;
        }
        return Filters.Aggregate(enumerable, (current, filter) => current.Where(filter));
    }
}

