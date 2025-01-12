using Streetcode.BLL.Repositories.Interfaces.Timeline;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Timeline;

namespace Streetcode.DAL.Repositories.Realizations.Timeline
{
    public class HistoricalContextTimelineRepository : RepositoryBase<HistoricalContextTimeline>, IHistoricalContextTimelineRepository
    {
        public HistoricalContextTimelineRepository(StreetcodeDbContext dbContext)
        : base(dbContext)
        {
        }
    }
}
