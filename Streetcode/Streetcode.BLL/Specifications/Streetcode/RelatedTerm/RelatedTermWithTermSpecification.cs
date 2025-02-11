﻿using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.DAL.Specification;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.Specifications.Streetcode.RelatedTerm
{
    public class RelatedTermWithTermSpecification : BaseSpecification<RelatedTermEntity>
    {
        public RelatedTermWithTermSpecification()
        {
            AddInclude(x => x.Term);
            AddCaching("AllRelatedTermsWithTerm");
        }

        public RelatedTermWithTermSpecification(int id)
            : base(rt => rt.Id == id)
        {
            AddInclude(x => x.Term);
        }

        public RelatedTermWithTermSpecification(RelatedTermDto relatedTermDTO)
            : base(rt => rt.TermId == relatedTermDTO.TermId && rt.Word == relatedTermDTO.Word)
        {
            AddInclude(x => x.Term);
        }
    }
}
