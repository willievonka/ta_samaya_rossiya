namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateLayerRegionStyleRequest(
    bool? Stroke,
    string? Color,
    int? Weight,
    double? Opacity,
    string? LineCap,
    string? LineJoin, 
    string? DashArray, 
    string? DashOffset,
    bool? Fill,
    string? FillColor, 
    double? FillOpacity,
    string? FillRule,
    string? ClassName
    );