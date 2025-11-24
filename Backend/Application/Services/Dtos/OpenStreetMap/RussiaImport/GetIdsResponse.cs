using Application.Services.Dtos.OpenStreetMap.SharedModels;

namespace Application.Services.Dtos.OpenStreetMap.RussiaImport;

public class GetIdsResponse : BaseOverpassApiResponse
{
    public IdElement[] Elements { get; set; }
}