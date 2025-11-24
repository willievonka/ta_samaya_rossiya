using System.Linq.Expressions;

namespace Application.Queries;

/// <summary>
/// Параметр включения других таблиц
/// </summary>
/// <typeparam name="TEntity">Сущность из бд</typeparam>
public class IncludeParams<TEntity> where TEntity : class
{
    public List<Expression<Func<TEntity, object?>>>? IncludeProperties { get; set; }
}