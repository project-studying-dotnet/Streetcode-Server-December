using Streetcode.BLL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.DAL.Repositories.Realizations.Media.Images
{
    public class ImageDetailsRepository : RepositoryBase<ImageDetails>, IImageDetailsRepository
    {
        public ImageDetailsRepository(StreetcodeDbContext dbContext)
        : base(dbContext)
        {
        }
    }
}
