using Application.Services.Dtos.OpenStreetMap.SharedModels;

namespace Application.Services.Dtos.OpenStreetMap.RegionSearch;

public class SearchRegionResponse : BaseOverpassApiResponse
{
    public SearchRegionItem[] Elements { get; set; } = null!;
}