using System.Text.Json;
using Domain.Entities;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LayerRegionConfiguration : IEntityTypeConfiguration<LayerRegion>
{
    public void Configure(EntityTypeBuilder<LayerRegion> builder)
    {
        builder.Property(lr => lr.Style)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, RegionStyleJsonConverter.Options),
                v => v == null ? null : JsonSerializer.Deserialize<LayerRegionStyle>(v, RegionStyleJsonConverter.Options)!
                )
            .Metadata.SetValueComparer(RegionStyleJsonConverter.Comparer);
    }
}