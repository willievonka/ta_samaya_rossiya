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
    /// Fk на Region
    /// </summary>
    public Guid RegionId { get; set; }
    
    /// <summary>
    /// Регион, которому принадлежит граница
    /// </summary>
    public Region Region { get; set; }
}