using Streetcode.BLL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.AdditionalContent;

namespace Streetcode.DAL.Repositories.Realizations.AdditionalContent
{
    public class StreetcodeTagIndexRepository : RepositoryBase<StreetcodeTagIndex>, IStreetcodeTagIndexRepository
    {
        public StreetcodeTagIndexRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}
