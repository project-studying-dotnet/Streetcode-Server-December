using Streetcode.DAL.Specification;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.BLL.Specifications.Comment
{
    public class CommentByParentIdSpecification : BaseSpecification<CommentEntity>
    {
        public CommentByParentIdSpecification(int parentId)
            : base(c => c.Id == parentId)
        {
        }
    }
}
