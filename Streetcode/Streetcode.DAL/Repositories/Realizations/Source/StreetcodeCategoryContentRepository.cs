using Streetcode.BLL.Repositories.Interfaces.Source;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Sources;

namespace Streetcode.DAL.Repositories.Realizations.Source
{
    public class StreetcodeCategoryContentRepository : RepositoryBase<StreetcodeCategoryContent>, IStreetcodeCategoryContentRepository
    {
        public StreetcodeCategoryContentRepository(StreetcodeDbContext dbContext)
        : base(dbContext)
        {
        }
    }
}
