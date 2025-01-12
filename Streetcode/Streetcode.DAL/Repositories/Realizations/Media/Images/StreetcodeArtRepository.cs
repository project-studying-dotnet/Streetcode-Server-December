using Streetcode.BLL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.DAL.Repositories.Realizations.Media.Images
{
    public class StreetcodeArtRepository : RepositoryBase<StreetcodeArt>, IStreetcodeArtRepository
    {
        public StreetcodeArtRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
