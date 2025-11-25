namespace Domain.Entities;

/// <summary>
/// Связь регионов и исторических линий
/// </summary>
public class LineRegion
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор исторической линии
    /// </summary>
    public Guid LineId { get; set; }

    /// <summary>
    /// Ссылка на Историческую линию
    /// </summary>
    public HistoricalLine Line { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор региона
    /// </summary>
    public Guid RegionId { get; set; }

    /// <summary>
    /// Ссылка на регион
    /// </summary>
    public Region Region { get; set; } = null!;
    
    /// <summary>
    /// Состояние региона на исторической линии (подсвечивается или нет)
    /// </summary>
    public bool IsActive { get; set; }
}