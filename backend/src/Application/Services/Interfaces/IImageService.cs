namespace Application.Services.Interfaces;

public interface IImageService
{
    //TODO формировать URI из path
    Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile);
    Task<string> UpdateImageAsync(Guid entityId, string? pathDeletingFile, string folder, IFormFile newFile);
}