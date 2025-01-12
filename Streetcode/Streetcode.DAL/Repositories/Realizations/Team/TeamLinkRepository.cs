using Streetcode.BLL.Repositories.Interfaces.Team;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.DAL.Repositories.Realizations.Team
{
    public class TeamLinkRepository : RepositoryBase<TeamMemberLink>, ITeamLinkRepository
    {
        public TeamLinkRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
