using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Request;

public record UpsertIndicatorsRequest(
    [FromForm] IFormFile? Image,
    int? ExcursionsCount,
    int? PartnersCount,
    int? MembersCount,
    bool? IsActive
    );