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
    
    public async Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile)
    {
        var path = await _saveImageService.SaveImageAsync(entityId, folder, newFile);
        _logger.LogInformation($"Image saved: {path}");
        
        return GetUriFromRelativePath(path);
    }

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

    private string GetUriFromRelativePath(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            _logger.LogError("Path is null or empty");
            throw new ArgumentNullException("Path is null or empty");
        }

        return relativePath.Replace("\\", "/").Insert(0, "/");
    }

    private string GetRelativePathFromUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            _logger.LogError("Uri is null or empty");
            throw new ArgumentNullException("Uri is null or empty");
        }
        
        return uri.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
    }
}