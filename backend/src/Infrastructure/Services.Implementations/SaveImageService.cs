using System.Text.RegularExpressions;
using Application.Services.Interfaces;

namespace Infrastructure.Services.Implementations;

public class SaveImageService : ISaveImageService
{
    private readonly IWebHostEnvironment _environment;

    private const string UploadsFolder = "images";
    
    public SaveImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(Guid entityId, string folder, IFormFile newFile)
    {
        if (newFile == null || newFile.Length == 0)
        {   
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
        
        var correctFileName = Regex.Replace(newFile.Name, @"\s+", "").Trim();
        var fileName = $"{entityId.ToString()}.{correctFileName}";
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
        var fullPath = Path.Combine(_environment.WebRootPath, UploadsFolder, path);
        
        if (string.IsNullOrEmpty(fullPath))
        {
            throw new ArgumentException("Invalid path");
        }
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", path);
        }
        
        await Task.Run(()=> File.Delete(fullPath));
    }
}