using Streetcode.BLL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Partners;

namespace Streetcode.DAL.Repositories.Realizations.Partners
{
    public class PartnerStreetodeRepository : RepositoryBase<StreetcodePartner>, IPartnerStreetcodeRepository
    {
        public PartnerStreetodeRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}
