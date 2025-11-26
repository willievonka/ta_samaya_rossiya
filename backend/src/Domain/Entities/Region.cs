using Domain.Enums;
using NetTopologySuite.Geometries;

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
    /// Является ли регион регионом РФ
    /// </summary>
    public bool IsRussia { get; set; }

    /// <summary>
    /// Тип региона
    /// </summary>
    public RegionType Type { get; set; }
}