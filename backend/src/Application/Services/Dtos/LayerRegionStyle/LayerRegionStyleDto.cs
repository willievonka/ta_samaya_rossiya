namespace Application.Services.Dtos.LayerRegionStyle;

public class LayerRegionStyleDto
{
    public bool? Stroke { get; set; }
    public string? Color { get; set; }
    public int? Weight { get; set; }
    public double? Opacity { get; set; }
    public string? LineCap { get; set; }
    public string? LineJoin { get; set; }
    public string? DashArray { get; set; }        
    public string? DashOffset { get; set; }
    public bool? Fill { get; set; }
    public string? FillColor { get; set; }
    public double? FillOpacity { get; set; }
    public string? FillRule { get; set; }
    public string? ClassName { get; set; }
}