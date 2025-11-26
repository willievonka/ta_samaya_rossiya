using System.Drawing;
using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;

namespace Domain.Entities;

public class HistoricalObject
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Координаты точки объекта на карте
    /// </summary>
    public Point Coordinates { get; set; }
    
    /// <summary>
    /// Порядковый номер объекта внутри исторической линии
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Название объекта
    /// </summary>
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Путь к изображению в локальном хранилище (images/objects/image.png)
    /// </summary>
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// Описание объекта
    /// </summary>
    public string Description { get; set; }  = "";
    
    /// <summary>
    /// Отображается ли регион для пользователей
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Идентификатор исторической линии, к которой принадлежит объект
    /// </summary>
    public Guid LineId { get; set; }

    /// <summary>
    /// Ссылка на историческую линию, которой принадлежит объект
    /// </summary>
    public HistoricalLine HistoricalLine { get; set; }
}