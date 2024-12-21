using Streetcode.DAL.Specification;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.Specifications.Streetcode.RelatedTerm
{
    public class RelatedTermByTermIdSpecification : BaseSpecification<RelatedTermEntity>
    {
        public RelatedTermByTermIdSpecification(int id)
            : base(rt => rt.TermId == id)
        {
        }
    }
}
