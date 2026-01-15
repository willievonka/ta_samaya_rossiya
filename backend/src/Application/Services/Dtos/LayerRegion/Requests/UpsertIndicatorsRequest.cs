using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Dtos.LayerRegion.Requests;

public record UpsertIndicatorsRequest(
    [FromForm] IFormFile? Image,
    int? ExcursionsCount,
    int? PartnersCount,
    int? MembersCount,
    bool? IsActive
    );