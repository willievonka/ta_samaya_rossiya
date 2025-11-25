using Domain.Entities;
using NetTopologySuite.Geometries;

namespace Domain.Services.Interfaces;

public interface IRegionСreationService
{
    Task<Region> CreateFromOsmAsync(int osmId, MultiPolygon geometry, string title, Guid? lineId, CancellationToken ct);
}