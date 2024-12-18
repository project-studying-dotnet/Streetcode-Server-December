using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streetcode.DAL.Persistence.Configurations.News;

public class NewsConfiguration : IEntityTypeConfiguration<Entities.News.News>
{
    public void Configure(EntityTypeBuilder<Entities.News.News> builder)
    {
        builder
            .HasOne(x => x.Image)
            .WithOne(x => x.News)
            .HasForeignKey<Entities.News.News>(x => x.ImageId);
    }
}