namespace WebApi.Controllers.AdminControllers.LayerRegionStyle.Response;

public record LayerRegionStyleResponse(
    bool? Stroke = null,
    string? Color = null,
    int? Weight = null,
    double? Opacity = null,
    string? LineCap = null,
    string? LineJoin = null, 
    string? DashArray = null, 
    string? DashOffset = null,
    bool? Fill = null,
    string? FillColor = null, 
    double? FillOpacity = null,
    string? FillRule = null,
    string? ClassName = null
    );