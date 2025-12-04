using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record UpdateMapRequest(
    [Required] bool IsAnalytics, 
    [Required] string Title,
    [Required] string Description,
    IFormFile? BackgroundImage,
    IEnumerable<CreateLayerRegionRequest>? Regions
    );