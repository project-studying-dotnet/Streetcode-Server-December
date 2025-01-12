using Streetcode.BLL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent
{
    public class TermRepository : RepositoryBase<Term>, ITermRepository
    {
        public TermRepository(StreetcodeDbContext streetcodeDbContext)
            : base(streetcodeDbContext)
        {
        }
    }
}