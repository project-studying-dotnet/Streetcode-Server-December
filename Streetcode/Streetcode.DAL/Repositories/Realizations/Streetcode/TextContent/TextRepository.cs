using Streetcode.BLL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent
{
    public class TextRepository : RepositoryBase<Text>, ITextRepository
    {
        public TextRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}