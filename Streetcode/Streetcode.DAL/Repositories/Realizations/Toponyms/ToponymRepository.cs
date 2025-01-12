using Streetcode.BLL.Repositories.Interfaces.Toponyms;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Toponyms;

namespace Streetcode.DAL.Repositories.Realizations.Toponyms
{
    public class ToponymRepository : RepositoryBase<Toponym>, IToponymRepository
    {
        public ToponymRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}