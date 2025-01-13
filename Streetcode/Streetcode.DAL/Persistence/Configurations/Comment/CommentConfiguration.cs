using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.DAL.Persistence.Configurations.Comment
{
    public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId);

            builder.HasOne(c => c.Streetcode)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.StreetcodeId);
        }
    }
}
