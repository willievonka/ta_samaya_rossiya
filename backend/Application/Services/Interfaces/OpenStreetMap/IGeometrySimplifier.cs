using NetTopologySuite.Geometries;

namespace Application.Services.Interfaces.OpenStreetMap;

public interface IGeometrySimplifier
{
    Task<Geometry> SimplifyToPercentageAsync(string geoJson, CancellationToken ct = default);
}