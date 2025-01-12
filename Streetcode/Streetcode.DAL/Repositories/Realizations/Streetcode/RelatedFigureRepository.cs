using Streetcode.BLL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.DAL.Repositories.Realizations.Streetcode
{
    public class RelatedFigureRepository : RepositoryBase<RelatedFigure>, IRelatedFigureRepository
    {
        public RelatedFigureRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}