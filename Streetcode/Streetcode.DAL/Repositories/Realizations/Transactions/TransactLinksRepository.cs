using Streetcode.BLL.Repositories.Interfaces.Transactions;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.Domain.Entities.Transactions;

namespace Streetcode.DAL.Repositories.Realizations.Transactions
{
    public class TransactLinksRepository : RepositoryBase<TransactionLink>, ITransactLinksRepository
    {
        public TransactLinksRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}