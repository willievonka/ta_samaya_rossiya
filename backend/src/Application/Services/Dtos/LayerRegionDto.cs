using NetTopologySuite.Geometries;

namespace Application.Services.Dtos;

public class LayerRegionDto
{
    public Guid? Id { get; set; }
    
    /// <summary>
    /// Цвет заливки региона
    /// </summary>
    public LayerRegionStyleDto? Style { get; set; }
    
    /// <summary>
    /// Активный ли регион, будем ли мы его отображать
    /// </summary>
    public required bool IsActive { get; set; }
    
    /// <summary>
    /// Название региона (для внутреннего отображения)
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Полигон, обозначающий границы региона
    /// </summary>
    public MultiPolygon? Geometry { get; set; }
    
    public IndicatorsRegionDto? Indicators { get; set; }
}