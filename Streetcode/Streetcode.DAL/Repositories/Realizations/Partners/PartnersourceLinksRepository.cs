using Streetcode.BLL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Partners;

namespace Streetcode.DAL.Repositories.Realizations.Partners
{
    public class PartnersourceLinksRepository : RepositoryBase<PartnerSourceLink>, IPartnerSourceLinkRepository
    {
        public PartnersourceLinksRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}
