using System.IO.Compression;
using System.Text;
using Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services.Implementations;

/// <summary>
/// Сервис для кеширования готовых json объектов.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromDays(21);
    
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Получить готовый json объект из кэша по ключу.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string?> GetCachedResponseAsync(string key, CancellationToken ct = default)
    {
        var compressedBytes = await _cache.GetAsync(key, ct);
        if (compressedBytes == null)
            return null;
        
        using var inputStream = new MemoryStream(compressedBytes);
        using var outputStream = new MemoryStream();

        using(var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        {
            await gZipStream.CopyToAsync(outputStream, ct);
        }
        
        outputStream.Position = 0;
        
        using var reader = new StreamReader(outputStream, Encoding.UTF8);
        return await reader.ReadToEndAsync(ct);
    }

    /// <summary>
    /// Кэшируем готовый json объект по ключу. expiration по умолчанию 21 день.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="json"></param>
    /// <param name="expiration"></param>
    /// <param name="ct"></param>
    public async Task SetCachedResponseAsync(string key, string json, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(json)) 
            return;
        
        var rawBytes = Encoding.UTF8.GetBytes(json);
        
        using var outputStream = new MemoryStream();

        using (var gZipStream = new GZipStream(outputStream, CompressionLevel.Optimal, true))
        {
            await gZipStream.WriteAsync(rawBytes, ct);
            await gZipStream.FlushAsync(ct);
        }
        
        byte[] compressedBytes = outputStream.ToArray();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };
        
        await _cache.SetAsync(key, compressedBytes, options, ct);
    }

    /// <summary>
    /// Удалить значение из кэша.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ct"></param>
    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
    }
}