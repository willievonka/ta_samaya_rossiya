namespace Domain.Entities;

/// <summary>
/// Регион на карте, может быть регионом РФ или другой цельной страной
/// </summary>
public class Region
{
    public Guid Id { get; set; }

    /// <summary>
    /// Название региона (для внутреннего отображения)
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Границы региона
    /// </summary>
    public RegionGeometry Geometry { get; set; }
}