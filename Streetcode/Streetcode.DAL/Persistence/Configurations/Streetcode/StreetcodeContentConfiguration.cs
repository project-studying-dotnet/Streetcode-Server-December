using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.Domain.Entities.Media.Images;
using Streetcode.Domain.Entities.Partners;
using Streetcode.Domain.Entities.Sources;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Streetcode.TextContent;
using Streetcode.Domain.Entities.Streetcode.Types;
using Streetcode.Domain.Entities.Toponyms;
using Streetcode.Domain.Entities.Transactions;
using Streetcode.Domain.Enums;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode
{
    public class StreetcodeContentConfiguration : IEntityTypeConfiguration<StreetcodeContent>
    {
        public void Configure(EntityTypeBuilder<StreetcodeContent> builder)
        {
            builder
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(s => s.ViewCount)
                .HasDefaultValue(0);

            builder
                .HasDiscriminator<string>(StreetcodeTypeDiscriminators.DiscriminatorName)
                .HasValue<StreetcodeContent>(StreetcodeTypeDiscriminators.StreetcodeBaseType)
                .HasValue<PersonStreetcode>(StreetcodeTypeDiscriminators.StreetcodePersonType)
                .HasValue<EventStreetcode>(StreetcodeTypeDiscriminators.StreetcodeEventType);

            builder
                .Property<string>("StreetcodeType").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            builder
                .HasMany(d => d.Coordinates)
                .WithOne(c => c.Streetcode)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.Facts)
                .WithOne(f => f.Streetcode)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.Images)
                .WithMany(i => i.Streetcodes)
                .UsingEntity<StreetcodeImage>(
                    si => si.HasOne(i => i.Image).WithMany().HasForeignKey(i => i.ImageId),
                    si => si.HasOne(i => i.Streetcode).WithMany().HasForeignKey(i => i.StreetcodeId))
                .ToTable("streetcode_image", "streetcode");

            builder
                .HasMany(d => d.TimelineItems)
                .WithOne(t => t.Streetcode)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.Toponyms)
                .WithMany(t => t.Streetcodes)
                .UsingEntity<StreetcodeToponym>(
                    st => st.HasOne(s => s.Toponym).WithMany().HasForeignKey(x => x.ToponymId),
                    st => st.HasOne(s => s.Streetcode).WithMany().HasForeignKey(x => x.StreetcodeId))
                .ToTable("streetcode_toponym", "streetcode");

            builder
                .HasMany(d => d.SourceLinkCategories)
                .WithMany(c => c.Streetcodes)
                .UsingEntity<StreetcodeCategoryContent>(
                    scat => scat.HasOne(i => i.SourceLinkCategory).WithMany(s => s.StreetcodeCategoryContents).HasForeignKey(i => i.SourceLinkCategoryId),
                    scat => scat.HasOne(i => i.Streetcode).WithMany(s => s.StreetcodeCategoryContents).HasForeignKey(i => i.StreetcodeId))
                .ToTable("streetcode_source_link_categories", "sources");

            builder
                .HasMany(d => d.Partners)
                .WithMany(p => p.Streetcodes)
                .UsingEntity<StreetcodePartner>(
                    sp => sp.HasOne(i => i.Partner).WithMany().HasForeignKey(x => x.PartnerId),
                    sp => sp.HasOne(i => i.Streetcode).WithMany().HasForeignKey(x => x.StreetcodeId))
                .ToTable("streetcode_partners", "streetcode");

            builder
                .HasMany(d => d.Videos)
                .WithOne(p => p.Streetcode)
                .HasForeignKey(d => d.StreetcodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(d => d.Audio)
                .WithOne(p => p.Streetcode)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(d => d.Text)
                .WithOne(p => p.Streetcode)
                .HasForeignKey<Text>(d => d.StreetcodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(d => d.TransactionLink)
                .WithOne(p => p.Streetcode)
                .HasForeignKey<TransactionLink>(d => d.StreetcodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(d => d.StatisticRecords)
                .WithOne(t => t.Streetcode)
                .HasForeignKey(t => t.StreetcodeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(x => x.TransliterationUrl)
                .IsUnique();

            builder
                .HasIndex(x => x.Index)
                .IsUnique();
        }
    }
}