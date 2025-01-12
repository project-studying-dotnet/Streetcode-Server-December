using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.Domain.Entities.Sources;

namespace Streetcode.DAL.Persistence.Configurations.Sources
{
    public class SourceLinkCategoryConfiguration : IEntityTypeConfiguration<SourceLinkCategory>
    {
        public void Configure(EntityTypeBuilder<SourceLinkCategory> builder)
        {
            builder
                .HasMany(d => d.StreetcodeCategoryContents)
                .WithOne(p => p.SourceLinkCategory)
                .HasForeignKey(d => d.SourceLinkCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}