namespace Domain.Entities;

/// <summary>
/// Карта (направление), может быть аналитическая
/// </summary>
public class Map
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Флаг для аналитической карты
    /// </summary>
    public bool? IsAnalytics { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Описание для карточки
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// Инфо текст для подробной информации карты
    /// </summary>
    public string Info { get; set; } = "";
    
    /// <summary>
    /// Путь к фоновому изображению для мини-карточки
    /// </summary>
    public string? BackgroundImage { get; set; }
    
    /// <summary>
    /// Цвет для активных слоёв региона, используются только на НЕ аналитических картах
    /// </summary>
    public string? ActiveLayerRegionsColor { get; set; }
    
    /// <summary>
    /// Цвет для маркеров исторических объектов, используются только на НЕ аналитических картах
    /// </summary>
    public string? HistoricalObjectPointColor { get; set; }
    
    /// <summary>
    /// Дата создания 
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Отображаемые регионы для этой карты
    /// </summary>
    public IEnumerable<LayerRegion> Regions { get; set; }
}