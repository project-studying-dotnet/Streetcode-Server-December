using Streetcode.BLL.Repositories.Interfaces.Analytics;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Analytics;

namespace Streetcode.DAL.Repositories.Realizations.Analytics
{
    public class StatisticRecordsRepository : RepositoryBase<StatisticRecord>, IStatisticRecordRepository
    {
        public StatisticRecordsRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}
