namespace Application.Services.Dtos;

public class IndicatorsRegionDto
{
    public Guid? Id { get; set; }
    
    /// <summary>
    /// Путь к изображению для всплывающего окна показателей региона
    /// </summary>
    public string? ImagePath { get; set; }
    
    public IFormFile? Image { get; set; }
    
    /// <summary>
    /// Кол-во экскурсий в регионе
    /// </summary>
    public required int Excursions { get; set; }
    
    /// <summary>
    /// Кол-во партнеров в регионе
    /// </summary>
    public required int Partners { get; set; }
    
    /// <summary>
    /// Кол-во участников в регионе
    /// </summary>
    public required int Participants { get; set; }
    
    /// <summary>
    /// Отображаются ли показатели пользователям
    /// </summary>
    public required bool IsActive { get; set; }
}