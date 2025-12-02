using Application.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers;

[ApiController]
[Route("api/images")]
public class ImageController : ControllerBase
{
    private readonly ISaveImageService  _saveImageService;
    private readonly IWebHostEnvironment _environment;

    public ImageController(ISaveImageService saveImageService, IWebHostEnvironment environment)
    {
        _saveImageService = saveImageService;
        _environment = environment;
    }

    /// <summary>
    /// Загрузка изображения
    /// </summary>
    /// <returns></returns>
    [HttpPost("{entityId:guid}")]
    public async Task<IActionResult> SaveImage([FromRoute] Guid entityId, string folder, IFormFile newFile)
    {
        var path = await _saveImageService.SaveImageAsync(entityId, folder, newFile);
        
        return Ok(path);
    }
    
    [HttpDelete("")]
    public async Task<IActionResult> DeleteImage(string path)
    { 
        await _saveImageService.DeleteImageAsync(path);
        return Ok();
    }

    [HttpGet("maps")]
    public async Task<IActionResult> GetMapCards()
    {
        var path = Path.Combine(_environment.WebRootPath, "/images/main-hub-cards/cards(2).json");
        
        var file = await System.IO.File.ReadAllTextAsync("D:\\RiderProjects\\TaSamayaRossiya\\backend\\src\\WebApi\\wwwroot\\images\\main-hub-cards\\cards(2).json");
        
        return Content(file, "application/json");
    }
}