using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateIndicatorsRequest(
    [FromForm] IFormFile? Image,
    int? ExcursionsCount,
    int? PartnersCount,
    int? MembersCount,
    bool? IsActive
    );