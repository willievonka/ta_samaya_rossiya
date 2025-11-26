using NetTopologySuite.Geometries;

namespace Domain.Entities;

/// <summary>
/// Сущность для хранения регионов
/// </summary>
public class RegionGeometry
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Полигон, обозначающий границы региона
    /// </summary>
    public MultiPolygon Geometry { get; set; }
    
    /// <summary>
    /// Дата создания 
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}