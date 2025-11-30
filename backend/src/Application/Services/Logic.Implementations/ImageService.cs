using Application.Services.Interfaces;

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
            await _saveImageService.DeleteImageAsync(pathDeletingFile);
        }
        
        var pathNewFile = await _saveImageService.SaveImageAsync(entityId, folder, newFile);
        _logger.LogInformation($"Image updated: {pathNewFile}");
        
        return GetUriFromRelativePath(pathNewFile);
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
}