namespace Domain.Entities;

/// <summary>
/// Показатели региона
/// </summary>
public class IndicatorsRegion
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор региона, к которому относятся показатели
    /// </summary>
    public Guid RegionId { get; set; }
    
    /// <summary>
    /// Ссылка на регионы, которым принадлежат показатели
    /// </summary>
    public Region Region { get; set; }
    
    /// <summary>
    /// Путь к изображению для всплывающего окна показателей региона
    /// </summary>
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// Кол-во экскурсий в регионе
    /// </summary>
    public int Excursions { get; set; }
    
    /// <summary>
    /// Кол-во партнеров в регионе
    /// </summary>
    public int Partners { get; set; }
    
    /// <summary>
    /// Кол-во участников в регионе
    /// </summary>
    public int Participants { get; set; }
}