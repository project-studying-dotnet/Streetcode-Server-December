using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streetcode.DAL.Persistence.Configurations.News
{
    public class NewsConfiguration : IEntityTypeConfiguration<Domain.Entities.News.News>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.News.News> builder)
        {
            builder
                .HasOne(x => x.Image)
                .WithOne(x => x.News)
                .HasForeignKey<Domain.Entities.News.News>(x => x.ImageId);
            builder
                .HasIndex(x => x.URL)
                .IsUnique();
        }
    }
}