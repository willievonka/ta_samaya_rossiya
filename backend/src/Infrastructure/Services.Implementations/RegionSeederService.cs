using System.Data;
using Application.Queries;
using Application.Services.Interfaces;
using Domain.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Infrastructure.Services.Implementations;

/// <summary>
/// Сервис для заполнения бд регионами с уже готовой картой.
/// </summary>
public class RegionSeederService : IRegionSeederService
{
    private readonly ICrudService _crudService;
    private readonly IQueryService _queryService;
    private readonly ILogger<RegionSeederService> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public RegionSeederService(ICrudService crudService, IQueryService queryService,
        ILogger<RegionSeederService> logger,  IWebHostEnvironment webHostEnvironment)
    {
        _crudService = crudService;
        _queryService = queryService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }
    
    public async Task SeedIfEmptyAsync(CancellationToken ct = default)
    {
        var regionsCount = await _queryService.GetCountAsync<Region>(new DataQueryParams<Region>(), ct);

        if (regionsCount > 0)
        {
            _logger.LogInformation("Table Regions already contains {count} entries.", regionsCount);
            return;
        }
        
        _logger.LogInformation("Table Regions is empty, starting seed.");

        var featureColletion = await ParseFeatureCollectionAsync(ct);

        var featureNumber = 0;
        var totalCount = featureColletion.Count;
        foreach (var feature in featureColletion)
        {
            featureNumber++;
            if (!feature.Attributes.Exists("NL_NAME_1_FIXED"))
            {
                _logger.LogInformation("Feature {Index} of {Total} is missing attribute NL_NAME_1_FIXED.", featureNumber, totalCount);
                continue;
            }

            MultiPolygon geometry;
            var featureGeometry = feature.Geometry;

            if (featureGeometry is MultiPolygon multiPolygon)
            {
                geometry = multiPolygon;
            }
            else if (featureGeometry is Polygon polygon)
            {
                geometry = new MultiPolygon(new [] { polygon });
            }
            else
            {
                _logger.LogError("Feature {Index} of {Total} has invalid type of geometry.", featureNumber, totalCount);
                throw new Exception($"Feature {featureNumber} of {totalCount} has invalid type of geometry.");
            }

            var gid0 = feature.Attributes["GID_0"]?.ToString();
            var name = feature.Attributes["NL_NAME_1_FIXED"]?.ToString();

            if (string.IsNullOrEmpty(gid0) || string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Feature {Index} of {Total} doesn't have \"NL_NAME_1_FIXED\" or \"GID_0\" properties.",
                    featureNumber, totalCount);
            }
            
            var regiondId = await _crudService.CreateAsync(new Region
            {
                Name = name ?? $"Новый регион №{featureNumber}",
                IsRussia = gid0 is "RUS" or "UKR",
                Geometry = new RegionGeometry { Geometry = geometry },
            }, ct);

            if (regiondId == Guid.Empty)
            {
                _logger.LogError("Couldn't create region from feature number {featureNumber} of {totalCount}.",
                    featureNumber, totalCount);
                throw new DataException($"Couldn't create region from feature number {featureNumber} of {totalCount}.");
            }
            
            _logger.LogInformation("Feature {Index} of {Total} finished seed.",  featureNumber, totalCount);
        }
    }

    /// <summary>
    /// Geojson должен соответствовать следующим требованиям:
    ///                   обязательное наличие:
    /// "NL_NAME_1_FIXED" в свойствах, значения пойдут в Name региона
    /// "GID_0" в свойствах, регион будет считаться Российским, если стоит значение UKR или RUS
    /// </summary>
    private async Task<FeatureCollection> ParseFeatureCollectionAsync(CancellationToken ct)
    {
        var path = Path.Combine(_webHostEnvironment.WebRootPath, "maps","map.geojson");
        var mapGeoJson = await File.ReadAllTextAsync(path, ct);
        
        var reader = new GeoJsonReader();
        var featureCollection = reader.Read<FeatureCollection>(mapGeoJson);

        if (featureCollection == null || !featureCollection.Any())
        {
            _logger.LogError("Invalid GeoJSON.");
            throw new InvalidOperationException("Invalid GeoJSON.");
        }
        
        return featureCollection;
    }
}