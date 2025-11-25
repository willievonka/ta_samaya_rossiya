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
    public bool IsAnalitics { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// Путь к фоновому изображению для мини-карточки
    /// </summary>
    public string? BackgroundImage { get; set; }
    
    /// <summary>
    /// Регионы для этой карты
    /// </summary>
    public IEnumerable<Region> Regions { get; set; }
}