using Application.Services.Colors;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Services.Interfaces;
using NetTopologySuite.Geometries;

namespace Infrastructure.Services.Implementations;

public class RegionCreationService : IRegionСreationService
{
    private readonly ICrudService _crudService;
    private readonly IColorService _colorService;

    public RegionCreationService(ICrudService crudService,  IColorService colorService)
    {
        _crudService = crudService;
        _colorService = colorService;
    }
    
    public Task<Region> CreateFromOsmAsync(int osmRegionId, MultiPolygon geometry, string title, Guid? lineId, CancellationToken ct)
    {
        throw new NotImplementedException("Not implemented yet");
        
        /*var region = new Region
        {
            OsmId = osmRegionId,
            Title = title,
            Border = geometry,
            DisplayTitle = title,
            DisplayTitleFontSize = 0,
            DisplayTitlePosition = geometry.Centroid,
            ShowDisplayTitle = true,
            FillColor = _colorService.GetRandomColorForRegion(),
            ShowIndicators = true,
            IsRussia = !lineId.HasValue
        };
        
        var indicators = new IndicatorsRegion { RegionId = region.Id };
        
        region.Id = await _crudService.CreateAsync(region, ct);
        await _crudService.CreateAsync(indicators, ct);
        
        if (lineId.HasValue)
        {
            await _crudService.CreateAsync(new LineRegion
            {
                LineId = lineId.Value,
                RegionId = region.Id,
                IsActive = true
            }, ct);
        }
        
        return region;*/
    }
}