using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.BLL.Specifications.Comment
{
    public class CommentWithChildrenSpecification : BaseSpecification<CommentEntity>
    {
        public CommentWithChildrenSpecification(int id)
            : base(rt => rt.Id == id)
        {
            AddInclude(x => x.Children!);
        }
    }
}
