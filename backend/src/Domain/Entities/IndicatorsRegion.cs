namespace Domain.Entities;

/// <summary>
/// Показатели региона
/// </summary>
public class IndicatorsRegion
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Fk на LayerRegion 
    /// </summary>
    public Guid RegionId { get; set; }
    
    /// <summary>
    /// Ссылка на реализацию региона, которому принадлежат показатели
    /// </summary>
    public LayerRegion Region { get; set; }
    
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
    
    /// <summary>
    /// Отображаются ли показатели пользователям
    /// </summary>
    public bool IsActive { get; set; }
}