using System.Text.Json;
using Application.Services.Common.Json;
using Application.Services.Interfaces;
using Application.Services.Logic.Interfaces;
using Application.Services.Mapper;

namespace Application.Services.Logic.Implementations;

/// <summary>
/// "Оркестратор" для кеширования и передачи запросов в бизнес логику. Используется непосредственно в контроллерах
/// </summary>
public class MapQueryCachingCachingService : IMapQueryCachingService
{
    private readonly ICacheService _cacheService;
    private readonly IMapService _mapService;
    private readonly ILogger<IMapQueryCachingService> _logger;

    public MapQueryCachingCachingService(ICacheService cacheService, IMapService mapService, ILogger<IMapQueryCachingService> logger)
    {
        _cacheService = cacheService;
        _mapService = mapService;
        _logger = logger;
    }

    /// <summary>
    /// Получить готовый json карты для отправки клиенту. Возможны Cache miss.
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string?> GetMapResponseAsync(Guid mapId, CancellationToken ct)
    {
        var cacheKey = $"map_{mapId}";
        
        var cachedJson = await _cacheService.GetCachedResponseAsync(cacheKey, ct);
        if (cachedJson != null) 
            return cachedJson;
        
        _logger.LogInformation("Cache miss for map {mapId}. Fetching from database.", mapId);
        
        var mapDto = await _mapService.GetMapAsync(mapId, ct);
        if (mapDto == null)
            return null;
        
        var responseDto = MapMapper.MapDtoToResponse(mapDto);
        
        var json = JsonSerializer.Serialize(responseDto, JsonCacheSettings.Default);
        
        await _cacheService.SetCachedResponseAsync(cacheKey, json, TimeSpan.FromDays(30), ct);
        
        return json;
    }

    /// <summary>
    /// Получить готовый json шаблона карты для отправки клиенту. Возможны Cache miss.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string?> GetEmptyMapResponseAsync(CancellationToken ct)
    {
        var cacheKey = "map_empty";
        
        var cachedJson = await _cacheService.GetCachedResponseAsync(cacheKey, ct);
        if (cachedJson != null) 
            return cachedJson;
        
        _logger.LogInformation("Cache miss for empty map. Fetching from database.");
        
        var emptyMap = await _mapService.GetEmptyMapAsync(ct);
        var responseDto = MapMapper.EmptyMapDtoToResponse(emptyMap);
        if (responseDto == null)
            return null;
        
        var json = JsonSerializer.Serialize(responseDto, JsonCacheSettings.Default);
        
        await _cacheService.SetCachedResponseAsync(cacheKey, json, TimeSpan.FromDays(90), ct);
        
        return json;
    }

    /// <summary>
    /// Получить готовый json карточек для отправки клиенту. Возможны Cache miss.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string> GetAllMapsCardsResponseAsync(CancellationToken ct)
    {
        var cacheKey = "maps_cards";
        
        var cachedJson = await _cacheService.GetCachedResponseAsync(cacheKey, ct);
        if (cachedJson != null) 
            return cachedJson;
        
        _logger.LogInformation("Cache miss for maps cards. Fetching from database.");
        
        var mapsCards = await _mapService.GetAllCardsAsync(ct);
        var responseDto = MapMapper.MapsDtosToMapsCardsResponse(mapsCards);
        
        var json = JsonSerializer.Serialize(responseDto, JsonCacheSettings.Default);
        
        await _cacheService.SetCachedResponseAsync(cacheKey, json, TimeSpan.FromDays(30), ct);
        
        return json;
    }

    /// <summary>
    /// Удалить кэш карты по её id.
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="ct"></param>
    public async Task RemoveMapResponseCacheAsync(Guid mapId, CancellationToken ct)
    {
        await _cacheService.RemoveAsync($"map_{mapId}", ct);
    }
    
    /// <summary>
    /// Удалить кэш шаблона карты.
    /// </summary>
    /// <param name="ct"></param>
    public async Task RemoveEmptyMapResponseCacheAsync(CancellationToken ct)
    {
        await _cacheService.RemoveAsync("map_empty", ct);
    }

    /// <summary>
    /// Удалить кэш карточек.
    /// </summary>
    /// <param name="ct"></param>
    public async Task RemoveAllMapsCardsResponseCacheAsync(CancellationToken ct)
    {
        await _cacheService.RemoveAsync("maps_cards", ct);
    }
}