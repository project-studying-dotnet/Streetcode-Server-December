using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
