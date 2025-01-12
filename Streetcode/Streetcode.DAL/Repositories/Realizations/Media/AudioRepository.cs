using Repositories.Interfaces;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Media;

namespace Streetcode.DAL.Repositories.Realizations.Media
{
    public class AudioRepository : RepositoryBase<Audio>, IAudioRepository
    {
        public AudioRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}