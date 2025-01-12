using Streetcode.BLL.Repositories.Interfaces.Team;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.DAL.Repositories.Realizations.Team
{
    public class TeamRepository : RepositoryBase<TeamMember>, ITeamRepository
    {
        public TeamRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
