using Streetcode.BLL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types;

namespace Streetcode.DAL.Repositories.Realizations.AdditionalContent
{
    public class StreetcodeCoordinateRepository : RepositoryBase<StreetcodeCoordinate>, IStreetcodeCoordinateRepository
    {
        public StreetcodeCoordinateRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}