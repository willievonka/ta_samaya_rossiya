namespace Application.Services.Interfaces;

public interface ISaveImageService
{
    Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile);
    Task DeleteImageAsync(string path);
}