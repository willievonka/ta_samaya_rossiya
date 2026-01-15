using Application.Services.Dtos.LayerRegion;

namespace Application.Services.Dtos.Map;

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
    public string? Title { get; set; }
    
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Инфо текст для подробной информации карты
    /// </summary>
    public string? Info { get; set; }  
    
    public string? ActiveLayerRegionsColor { get; set; }
    
    public string? HistoricalObjectPointColor { get; set; }
    
    /// <summary>
    /// Для загрузки изображения
    /// </summary>
    public IFormFile? BackgroundImage { get; set; }
    
    public string? BackgroundImagePath { get; set; }
    
    public IEnumerable<LayerRegionDto>? Regions { get; set; }
}