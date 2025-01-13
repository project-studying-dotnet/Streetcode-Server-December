using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.BLL.Specifications.Comment
{
    public class CommentByStreetcodeIdSpecification : BaseSpecification<CommentEntity>
    {
        public CommentByStreetcodeIdSpecification(int id)
            : base(rt => rt.StreetcodeId == id && rt.ParentId == null)
        {
            AddInclude(rt => rt.Children!);
            AddCaching($"Comments-StreetcodeContent-Id{id}");
            AddCachingTime(1);
        }
    }
}
