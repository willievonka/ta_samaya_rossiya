using Point = NetTopologySuite.Geometries.Point;

namespace Application.Services.Dtos;

public class HistoricalObjectDto
{
    public Guid? Id { get; set; }
    
    public Guid? LayerRegionId { get; set; }
    
    /// <summary>
    /// Координаты точки объекта на карте
    /// </summary>
    public Point Coordinates { get; set; }
    
    /// <summary>
    /// Год исторического объекта 
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Название объекта
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Описание объекта
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Путь к изображению в локальном хранилище (images/objects/image.png)
    /// </summary>
    public string ImagePath { get; set; }
    
    /// <summary>
    /// Для загрузки изображения
    /// </summary>
    public IFormFile? Image { get; set; }
    
    /// <summary>
    /// Ссылка на видоэкскурсию по объекту
    /// </summary>
    public string? ExcursionUrl { get; set; }
}