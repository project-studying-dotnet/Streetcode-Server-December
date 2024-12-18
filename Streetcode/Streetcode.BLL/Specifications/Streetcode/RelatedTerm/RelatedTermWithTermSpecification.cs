using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.Specifications.Streetcode.RelatedTerm
{
    public class RelatedTermWithTermSpecification : BaseSpecification<RelatedTermEntity>
    {
        public RelatedTermWithTermSpecification()
        {
            AddInclude(x => x.Term);
        }

        public RelatedTermWithTermSpecification(int id)
            : base(rt => rt.Id == id)
        {
            AddInclude(x => x.Term);
        }

        public RelatedTermWithTermSpecification(RelatedTermDTO relatedTermDTO)
            : base(rt => rt.TermId == relatedTermDTO.TermId && rt.Word == relatedTermDTO.Word)
        {
            AddInclude(x => x.Term);
        }
    }
}
