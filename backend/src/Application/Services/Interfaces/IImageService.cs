namespace Application.Services.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile);
    Task<string> UpdateImageAsync(Guid entityId, string? pathDeletingFile, string folder, IFormFile newFile);
}