using Streetcode.BLL.Repositories.Interfaces.Source;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Sources;

namespace Streetcode.DAL.Repositories.Realizations.Source
{
    public class SourceCategoryRepository : RepositoryBase<SourceLinkCategory>, ISourceCategoryRepository
    {
        public SourceCategoryRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}