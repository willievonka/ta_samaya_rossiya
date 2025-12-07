namespace Application.Services.Dtos;

public class MapDto
{
    public Guid? Id { get; set; }
    
    /// <summary>
    /// Флаг аналитической карты
    /// </summary>
    public bool? IsAnalytics { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Описание
    /// </summary>
    public required string Description { get; set; }
    
    public IFormFile? BackgroundImage { get; set; }
    
    public string? BackgroundImagePath { get; set; }
    
    public IEnumerable<LayerRegionDto>? Regions { get; set; }
}