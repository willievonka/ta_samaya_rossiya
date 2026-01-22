using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Infrastructure.Services.Implementations;

/// <summary>
/// Сервис для заполнения бд регионами с уже готовой картой.
/// </summary>
public class RegionSeederService : IRegionSeederService
{
    private readonly IRegionRepository _regionRepository;
    private readonly ILogger<RegionSeederService> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMapRepository _mapRepository;
    private readonly ILayerRegionRepository _layerRegionRepository;
    
    public RegionSeederService(IRegionRepository regionRepository,
        ILogger<RegionSeederService> logger,  IWebHostEnvironment webHostEnvironment,
        IMapRepository mapRepository, ILayerRegionRepository layerRegionRepository)
    {
        _regionRepository = regionRepository;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _mapRepository = mapRepository;
        _layerRegionRepository = layerRegionRepository;
    }
    
    /// <summary>
    /// Обновляет базовые регионы на основе map.geojson
    /// </summary>
    /// <param name="ct"></param>
    /// <exception cref="Exception"></exception>
    public async Task SeedNewRegionAsync(CancellationToken ct = default)
    {
        var regions = await _regionRepository.GetAllAsync(ct);

        if (regions?.Count > 0)
        {
            _logger.LogInformation("Table Regions already contains {count} entries.", regions.Count);
        }
        
        _logger.LogInformation("Starting seed in the table Regions.");

        var featureColletion = await ParseFeatureCollectionAsync(ct);
        
        await RemoveInactiveRegionsInAllMapsAsync(regions, featureColletion, ct);
        
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
            
            var name = feature.Attributes["NL_NAME_1_FIXED"]?.ToString();

            if (regions!.Any(r => r.Name == name))
            {
                _logger.LogInformation("Region {name} already exists.", name);
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
                geometry = new MultiPolygon([polygon]);
            }
            else
            {
                _logger.LogError("Feature {Index} of {Total} has invalid type of geometry.", featureNumber, totalCount);
                throw new Exception($"Feature {featureNumber} of {totalCount} has invalid type of geometry.");
            }

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Feature {Index} of {Total} doesn't have \"NL_NAME_1_FIXED\" properties.",
                    featureNumber, totalCount);
            }
            
            await _regionRepository.AddAsync(new Region
            {
                Name = name ?? $"Новый регион №{featureNumber}",
                Geometry = new RegionGeometry { Geometry = geometry },
            }, ct);
            
            _logger.LogInformation("Region {name} with index {Index} of {Total} finished seed.", name, featureNumber, totalCount);
        }

        await AddNewRegionInAllMapsAsync(ct);
    }

    /// <summary>
    /// Удаляет старые базовые регионы, которых нет в map.geojson, но которые есть базе
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="featureColletion"></param>
    /// <param name="ct"></param>
    private async Task RemoveInactiveRegionsInAllMapsAsync(List<Region>? regions, FeatureCollection featureColletion, CancellationToken ct)
    {
        _logger.LogInformation("Removing inactive regions from the table.");
        
        var namesInFeatures = featureColletion
            .Select(f => f.Attributes["NL_NAME_1_FIXED"]?.ToString())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToHashSet();
        
        var regionsToDelete = regions!
            .Where(r => !namesInFeatures.Contains(r.Name))
            .ToList();

        _logger.LogWarning("Found {count} regions to delete (not present in FeatureCollection).", regionsToDelete.Count);
        
        foreach (var region in regionsToDelete)
        {
            await _regionRepository.DeleteByIdAsync(region.Id, ct);
            _logger.LogWarning("Region {region} has been deleted.", region.Name);
        }
    }
    
    /// <summary>
    /// Добавляет новые слои регионов к каждой карте, на основе новых базовых регионов.
    /// </summary>
    /// <param name="ct"></param>
    private async Task AddNewRegionInAllMapsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting adding new regions in all maps.");
        
        var regions = await _regionRepository.GetAllAsync(ct);
        var maps = await _mapRepository.GetAllHeadersAsync(ct);

        foreach (var map in maps!)
        {
            var existingRegionsMap = await _layerRegionRepository.GetAllWithRegionAndGeometryByMapIdAsync(map.Id, ct);
            var namesExistingRegions = existingRegionsMap!.Select(lr => lr.Region.Name);
            
            var newRegions = regions!.ExceptBy(namesExistingRegions!, r => r.Name).ToList();
            
            _logger.LogInformation("Starting adding new regions in Map {map.Title}.", map.Title);
            
            foreach (var newRegion in newRegions)
            {
                await _layerRegionRepository.AddAsync(new LayerRegion
                {
                    MapId = map.Id,
                    IsActive = false,
                    RegionId = newRegion.Id
                }, ct);
                _logger.LogInformation("Region {region} has been added in Map {map.Title}", newRegion.Name, map.Title);
            }
        }
    }

    /// <summary>
    /// Geojson должен соответствовать следующим требованиям:
    ///                   обязательное наличие:
    /// "NL_NAME_1_FIXED" в свойствах, значения пойдут в Name региона
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