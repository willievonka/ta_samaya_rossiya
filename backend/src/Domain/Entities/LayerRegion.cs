namespace Domain.Entities;

/// <summary>
/// Сущность для отображения региона на карте со всеми составляющими
/// </summary>
public class LayerRegion
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Leaflet свойства для стилизации
    /// </summary>
    public LayerRegionStyle? Style { get; set; }
    
    /// <summary>
    /// Ссылка на показатели региона
    /// </summary>
    public IndicatorsRegion? Indicators { get; set; }
    
    /// <summary>
    /// Активный ли регион, будем ли мы его отображать
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Fk на регион
    /// </summary>
    public Guid RegionId { get; set; }
    
    /// <summary>
    /// Ссылка на регион
    /// </summary>
    public Region Region { get; set; }
    
    /// <summary>
    /// Fk на таблицу карты
    /// </summary>
    public Guid MapId { get; set; }
    
    /// <summary>
    /// Ссылка на карту
    /// </summary>
    public Map Map { get; set; }
}