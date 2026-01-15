namespace Application.Services.Dtos.Indicators;

public class IndicatorsRegionDto
{
    public Guid? Id { get; set; }
    
    /// <summary>
    /// Путь к изображению для всплывающего окна показателей региона
    /// </summary>
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// Для загрузки изображения
    /// </summary>
    public IFormFile? Image { get; set; }
    
    /// <summary>
    /// Кол-во экскурсий в регионе
    /// </summary>
    public int? Excursions { get; set; }
    
    /// <summary>
    /// Кол-во партнеров в регионе
    /// </summary>
    public int? Partners { get; set; }
    
    /// <summary>
    /// Кол-во участников в регионе
    /// </summary>
    public int? Participants { get; set; }
    
    /// <summary>
    /// Отображаются ли показатели пользователям
    /// </summary>
    public bool? IsActive { get; set; }
}