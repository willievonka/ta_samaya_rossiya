using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateLayerRegionRequest(
    [Required] string FillColor,
    [Required] bool IsActive,
    [Required] string Name,
    [Required] bool IsRussia,
    CreateIndicatorsRequest? Indicators = null
);