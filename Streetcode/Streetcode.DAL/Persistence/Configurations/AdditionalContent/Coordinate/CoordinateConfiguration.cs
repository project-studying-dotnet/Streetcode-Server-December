using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types;

namespace Streetcode.DAL.Persistence.Configurations.AdditionalContent.Coordinate
{
    public class CoordinateConfiguration : IEntityTypeConfiguration<Streetcode.Domain.Entities.AdditionalContent.Coordinates.Coordinate>
    {
        public void Configure(EntityTypeBuilder<Streetcode.Domain.Entities.AdditionalContent.Coordinates.Coordinate> builder)
        {
            builder
                .HasDiscriminator<string>("CoordinateType")
                .HasValue<Streetcode.Domain.Entities.AdditionalContent.Coordinates.Coordinate>("coordinate_base")
                .HasValue<StreetcodeCoordinate>("coordinate_streetcode")
                .HasValue<ToponymCoordinate>("coordinate_toponym");
        }
    }
}