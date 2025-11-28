using Application.Queries;
using Application.Services.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EF = Microsoft.EntityFrameworkCore.EF;

namespace Infrastructure.Services.Implementations;

/// <summary>
/// Сервис для базовых операций(создание, удаления, обновление, поиск)
/// </summary>
public class CrudService : ICrudService
{
    private readonly MapDbContext _mapDbContext;

    public CrudService(MapDbContext mapDbContext)
    {
        _mapDbContext = mapDbContext;
    }
    
    /// <summary>
    /// Получить объект из базы данных по конкретному идентификатору id
    /// </summary>
    /// <param name="id">Идентификатор объекта в базе данных.</param>
    /// <param name="includeParams">Параметры для include полей из базы данных.</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>
    /// Возвращается объект с указанным id, если он был найден в базе данных. Иначе - null.
    /// </returns>
    public async Task<T?> GetByIdOrDefaultAsync<T>(Guid id, IncludeParams<T> includeParams, CancellationToken ct) where T : class
    {
        var set = _mapDbContext.Set<T>().AsQueryable();
        set = ApplyIncludeParams(set, includeParams);
        return await set.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id ,ct);
    }

    /// <summary>
    /// Получить объект из базы данных по конкретному идентификатору id
    /// </summary>
    /// <param name="id">Идентификатор объекта в базе данных.</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>
    /// Возвращается объект с указанным id, если он был найден в базе данных. Иначе - null.
    /// </returns>
    public async Task<T?> GetByIdOrDefaultAsync<T>(Guid id, CancellationToken ct) where T : class
    {
        return await _mapDbContext.Set<T>().FindAsync(new object[] { id }, ct);
    }

    /// <summary>
    /// Удаляет сущность с указанным id из базы данных, если она существует.
    /// </summary>
    /// <param name="id">Идентификатор объекта в базе данных.</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>
    /// True - объект был найден и удален из базы данных.
    /// False - объект с указанным id не был найден в базе данных.
    /// </returns>
    public async Task<bool> TryRemoveAsync<T>(Guid id, CancellationToken ct) where T : class
    {
        var set = _mapDbContext.Set<T>();
        var entity = await set.FindAsync(new object[] { id }, ct);
        if (entity == null)
            return false;
        
        set.Remove(entity);
        await _mapDbContext.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Удаляет сущности с указанными id из базы данных, если они существуют.
    /// </summary>
    /// <param name="ids">Идентификаторы объектов в базе данных.</param>
    /// <param name="ct">Токен отмены</param>
    public async Task RemoveRangeAsync<T>(CancellationToken ct, params Guid[] ids) where T : class
    {
        var set = _mapDbContext.Set<T>();
        
        var entites = await  _mapDbContext.Set<T>()
            .Where(e => ids.Contains(EF.Property<Guid>(e, "Id")))
            .ToArrayAsync(ct);
        
        set.RemoveRange(entites);
        await _mapDbContext.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Удаляет сущности из базы данных.
    /// </summary>
    /// <param name="entities">Объекты в базе данных.</param>
    /// <param name="ct">Токен отмены</param>
    public async Task RemoveRangeAsync<T>(CancellationToken ct, params T[] entities) where T : class
    {
        var set = _mapDbContext.Set<T>();
        set.RemoveRange(entities);
        await _mapDbContext.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Добавляет сущность в соответствующую таблицу.
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="ct">Токен отмены</param>
    public async Task<Guid> CreateAsync<T>(T entity, CancellationToken ct) where T : class
    {
        var set = _mapDbContext.Set<T>();
        await set.AddAsync(entity, ct);
        await _mapDbContext.SaveChangesAsync(ct);
        var property = typeof(T).GetProperty("Id");
        return (Guid)property.GetValue(entity);
    }

    /// <summary>
    /// Обновляет поля существующей сущности.
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="ct">Токен отмены</param>
    public async Task<Guid> UpdateAsync<T>(T entity, CancellationToken ct) where T : class
    {
        var set = _mapDbContext.Set<T>();
        set.Update(entity);
        await _mapDbContext.SaveChangesAsync(ct);
        var property = typeof(T).GetProperty("Id");
        return (Guid)property.GetValue(entity);
    }
    
    /// <summary>
    /// Включение других таблиц
    /// </summary>
    /// <param name="set">query set</param>
    /// <param name="includeParams">Параметры для включения</param>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <returns>query set</returns>
    private IQueryable<T> ApplyIncludeParams<T>(IQueryable<T> set, IncludeParams<T> includeParams) where T : class
    {
        if (includeParams?.IncludeProperties?.Count > 0)
            foreach (var includeProp in includeParams.IncludeProperties)
                set = set.Include(includeProp);
        
        return set;
    }
}