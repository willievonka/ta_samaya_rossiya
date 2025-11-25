using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Историческая линия
/// </summary>
public class HistoricalLine
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название исторической линии
    /// </summary>
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Путь к изображению маркера объектов в локальном хранилище (images/markers/default_marker.png)
    /// </summary>
    public string? MarkerImagePath { get; set; } 
    
    /// <summary>
    /// Цвет линии, отображаемой на карте
    /// </summary>
    public string LineColor { get; set; } = "";
    
    /// <summary>
    /// Стиль линии, отображаемой на карте
    /// </summary>
    public LineStyle LineStyle { get; set; }
    
    /// <summary>
    /// Легенда (подпись) маркеров в исторической линии
    /// </summary>
    public string MarkerLegend { get; set; } = "";
    
    /// <summary>
    /// Доступна ли для пользователей данная историческая линия
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Время последнего изменения исторической линии
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}