using Streetcode.BLL.Repositories.Interfaces.Comment;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Analytics;
using Streetcode.DAL.Repositories.Realizations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.DAL.Repositories.Realizations.Comment
{
    public class CommentRepository : RepositoryBase<CommentEntity>, ICommentRepository
    {
        public CommentRepository(StreetcodeDbContext context, IRedisCacheService redisCacheService)
            : base(context, redisCacheService)
        {
        }
    }
}
