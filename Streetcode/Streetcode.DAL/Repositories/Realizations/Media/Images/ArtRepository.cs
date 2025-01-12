using Repositories.Interfaces;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.DAL.Repositories.Realizations.Media.Images
{
    public class ArtRepository : RepositoryBase<Art>, IArtRepository
    {
        public ArtRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
