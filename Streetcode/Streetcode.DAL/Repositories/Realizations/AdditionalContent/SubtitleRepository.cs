using Streetcode.BLL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.AdditionalContent;

namespace Streetcode.DAL.Repositories.Realizations.AdditionalContent
{
    public class SubtitleRepository : RepositoryBase<Subtitle>, ISubtitleRepository
    {
        public SubtitleRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}