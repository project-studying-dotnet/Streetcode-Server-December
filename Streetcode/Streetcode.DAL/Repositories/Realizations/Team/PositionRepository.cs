using Streetcode.BLL.Repositories.Interfaces.Team;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.DAL.Repositories.Realizations.Team
{
    public class PositionRepository : RepositoryBase<Positions>, IPositionRepository
    {
        public PositionRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
