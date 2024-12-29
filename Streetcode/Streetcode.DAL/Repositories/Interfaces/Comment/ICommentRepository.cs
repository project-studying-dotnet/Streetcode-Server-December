using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.DAL.Repositories.Interfaces.Comment
{
    public interface ICommentRepository : IRepositoryBase<CommentEntity>
    {
    }
}
