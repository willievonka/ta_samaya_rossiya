using NetTopologySuite.Geometries;

namespace Domain.Entities;

/// <summary>
/// Регион на карте, может быть регионом РФ или другой страной
/// </summary>
public class Region
{
    public Guid Id { get; set; }

    /// <summary>
    /// Id в OpenStreetMap
    /// </summary>
    public int OsmId { get; set; }

    /// <summary>
    /// Название региона (для внутреннего отображения)
    /// </summary>
    public string Title { get; set; } = "";
    
    // TODO сделать отдельную таблицу для геометрии, чтобы хранить координаты отдельно, привязывать к сущности региона.
    /// <summary>
    /// Полигон, обозначающий границы региона
    /// </summary>
    public MultiPolygon Border { get; set; }

    /// <summary>
    /// Отображаемое на карте название региона
    /// </summary>
    public string? DisplayTitle { get; set; }
    
    /// <summary>
    /// Размер шрифта отображаемого на карте названия региона
    /// </summary>
    public int DisplayTitleFontSize { get; set; }
    
    /// <summary>
    /// Метоположение отображаемого на карте названия региона
    /// </summary>
    public Point DisplayTitlePosition { get; set; }

    /// <summary>
    /// Нужно ли отображать на карте название региона из DisplayTitle
    /// </summary>
    public bool ShowDisplayTitle { get; set; }
    
    /// <summary>
    /// Цвет заливки региона
    /// </summary>
    public string FillColor { get; set; } = "";
    
    /// <summary>
    /// Нужно ли отображать показатели региона по клику на него во вкладке Показатели
    /// </summary>
    public bool ShowIndicators { get; set; }
    
    /// <summary>
    /// Является ли регион регионом РФ
    /// </summary>
    public bool IsRussia { get; set; }
    
    /// <summary>
    /// Карты для этого региона
    /// </summary>
    public IEnumerable<Map> Maps { get; set; }
}