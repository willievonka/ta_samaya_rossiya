using Application.Services.Interfaces;

namespace Infrastructure.Services.Implementations;

public class SaveImageService : ISaveImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ISaveImageService> _logger;
    
    private const string UploadsFolder = "images";
    
    public SaveImageService(IWebHostEnvironment environment, ILogger<ISaveImageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile)
    {
        if (newFile.Length == 0)
        {   
            _logger.LogError("The file has not been transferred or is empty");
            throw new ArgumentException("The file has not been transferred or is empty");
        }

        if (string.IsNullOrEmpty(folder))
        {
            folder = $"newFolder.{DateTime.Today}";
        }
        
        var folderPath = Path.Combine(_environment.WebRootPath, UploadsFolder, folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        var correctFileName = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(newFile.FileName);
        var fileName = $"{entityId.ToString()}.{correctFileName}{extension}";
        var fullPath = Path.Combine(folderPath, fileName);

        var relativePath = Path.Combine(UploadsFolder, folder, fileName);
        
        if (File.Exists(fullPath))
        {
            return relativePath;
        }
        
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await newFile.CopyToAsync(stream);
        }
        
        return relativePath;
    }
    
    public async Task DeleteImageAsync(string path)
    {
        var fullPath = Path.Combine(_environment.WebRootPath, path);
        
        if (string.IsNullOrEmpty(fullPath))
        {
            _logger.LogError("Invalid path");
        }
        
        if (!File.Exists(fullPath))
        {
            _logger.LogError("File not found");
        }
        
        await Task.Run(()=> File.Delete(fullPath));
    }
}