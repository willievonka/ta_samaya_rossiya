using System.Drawing;
using Domain.Enums;

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
    /// Ссылка на видеоэкскурсию для встраивания
    /// </summary>
    public string? VideoUrl { get; set; }
    
    /// <summary>
    /// Идентификатор исторической линии, к которой принадлежит объект
    /// </summary>
    public Guid LineId { get; set; }

    /// <summary>
    /// Ссылка на историческую линию, которой принадлежит объект
    /// </summary>
    public HistoricalLine HistoricalLine { get; set; }
}