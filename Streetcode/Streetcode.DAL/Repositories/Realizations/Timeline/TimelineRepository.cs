using Streetcode.BLL.Repositories.Interfaces.Timeline;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Timeline;

namespace Streetcode.DAL.Repositories.Realizations.Timeline
{
    public class TimelineRepository : RepositoryBase<TimelineItem>, ITimelineRepository
    {
        public TimelineRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}