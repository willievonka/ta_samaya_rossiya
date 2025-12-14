using Point = NetTopologySuite.Geometries.Point;

namespace Domain.Entities;

public class HistoricalObject
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Координаты точки объекта на карте
    /// </summary>
    public Point Coordinates { get; set; }
    
    /// <summary>
    /// Год исторического объекта 
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Название объекта
    /// </summary>
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Описание объекта
    /// </summary>
    public string Description { get; set; }  = "";
    
    /// <summary>
    /// Путь к изображению в локальном хранилище (images/objects/image.png)
    /// </summary>
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// Ссылка на видоэкскурсию по объекты
    /// </summary>
    public string? ExcursionUrl { get; set; }
    
    /// <summary>
    /// Идентификатор исторической линии, к которой принадлежит объект
    /// </summary>
    public Guid LayerRegionId { get; set; }

    /// <summary>
    /// Ссылка на историческую линию, которой принадлежит объект
    /// </summary>
    public LayerRegion LayerRegion { get; set; }
}