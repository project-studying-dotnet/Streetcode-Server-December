using Streetcode.BLL.Repositories.Interfaces.Newss;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.News;

namespace Streetcode.DAL.Repositories.Realizations.Newss
{
    public class NewsRepository : RepositoryBase<News>, INewsRepository
    {
        public NewsRepository(StreetcodeDbContext dbContext)
        : base(dbContext)
        {
        }
    }
}
