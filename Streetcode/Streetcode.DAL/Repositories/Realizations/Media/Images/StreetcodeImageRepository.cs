using Streetcode.BLL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.DAL.Repositories.Realizations.Media.Images
{
    public class StreetcodeImageRepository : RepositoryBase<StreetcodeImage>, IStreetcodeImageRepository
	{
		public StreetcodeImageRepository(StreetcodeDbContext context)
			: base(context)
		{
		}
	}
}
