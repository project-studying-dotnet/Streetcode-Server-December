using Streetcode.BLL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent
{
    public class RelatedTermRepository : RepositoryBase<RelatedTerm>, IRelatedTermRepository
    {
        public RelatedTermRepository(StreetcodeDbContext streetcodeDbContext, IRedisCacheService redisCacheService)
        : base(streetcodeDbContext, redisCacheService)
        {
        }
    }
}
