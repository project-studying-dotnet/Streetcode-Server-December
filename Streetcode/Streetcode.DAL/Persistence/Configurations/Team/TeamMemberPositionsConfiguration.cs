using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations.Team
{
    public class TeamMemberPositionsConfiguration : IEntityTypeConfiguration<TeamMemberPositions>
    {
        public void Configure(EntityTypeBuilder<TeamMemberPositions> builder)
        {
            builder
                .HasKey(nameof(TeamMemberPositions.TeamMemberId), nameof(TeamMemberPositions.PositionsId));
        }
    }
}