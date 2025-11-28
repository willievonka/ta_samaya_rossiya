namespace Domain.Entities;

/// <summary>
/// Сущность для отображения региона на карте со всеми составляющими
/// </summary>
public class LayerRegion
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Цвет заливки региона
    /// </summary>
    public string FillColor { get; set; } = "";
    
    /// <summary>
    /// Ссылка на показатели региона
    /// </summary>
    public IndicatorsRegion? Indicators { get; set; }
    
    /// <summary>
    /// Fk на регион
    /// </summary>
    public Guid RegionId { get; set; }
    
    /// <summary>
    /// Ссылка на регион
    /// </summary>
    public Region Region { get; set; }
    
    /// <summary>
    /// Fk на таблиуц карты
    /// </summary>
    public Guid MapId { get; set; }
    
    /// <summary>
    /// Ссылка на карту
    /// </summary>
    public Map Map { get; set; }
}