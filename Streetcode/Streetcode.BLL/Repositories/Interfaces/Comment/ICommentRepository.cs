using Streetcode.BLL.Repositories.Interfaces.Base;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.BLL.Repositories.Interfaces.Comment
{
    public interface ICommentRepository : IRepositoryBase<CommentEntity>
    {
    }
}
