using Application.Services.Interfaces;
using Application.Services.Logic.Interfaces;

namespace Application.Services.Logic.Implementations;

public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;
    private readonly ISaveImageService _saveImageService;

    public ImageService(ILogger<ImageService> logger, ISaveImageService saveImageService)
    {
        _logger = logger;
        _saveImageService = saveImageService;
    }
    
    /// <summary>
    /// Сохраняет изображение
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="folder"></param>
    /// <param name="newFile"></param>
    /// <returns></returns>
    public async Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile)
    {
        var path = await _saveImageService.SaveImageAsync(entityId, folder, newFile);
        _logger.LogInformation($"Image saved: {path}");
        
        return GetUriFromRelativePath(path);
    }

    /// <summary>
    /// Обновляет изображение, старое удаляется из системы
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="pathDeletingFile"></param>
    /// <param name="folder"></param>
    /// <param name="newFile"></param>
    /// <returns></returns>
    public async Task<string> UpdateImageAsync(Guid entityId, string? pathDeletingFile, string folder, IFormFile newFile)
    {
        if (!string.IsNullOrEmpty(pathDeletingFile))
        {
            var relativePath = GetRelativePathFromUri(pathDeletingFile);
            
            await _saveImageService.DeleteImageAsync(relativePath);
        }
        
        var pathNewFile = await _saveImageService.SaveImageAsync(entityId, folder, newFile);
        _logger.LogInformation($"Image updated: {pathNewFile}");
        
        return GetUriFromRelativePath(pathNewFile);
    }
    
    public async Task DeleteImageAsync(string path)
    {
        var relativePath = GetRelativePathFromUri(path);
        
        await _saveImageService.DeleteImageAsync(relativePath);
    }

    /// <summary>
    /// Создаёт URI на основе относительного пути
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private string GetUriFromRelativePath(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            _logger.LogError("Path is null or empty");
            throw new ArgumentNullException(nameof(relativePath), "Path is null or empty");
        }

        return relativePath.Replace("\\", "/").Insert(0, "/");
    }

    /// <summary>
    /// Создаёт относительный путь на основе URI
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private string GetRelativePathFromUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            _logger.LogError("Uri is null or empty");
            throw new ArgumentNullException(nameof(uri), "Uri is null or empty");
        }
        
        return uri.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
    }
}