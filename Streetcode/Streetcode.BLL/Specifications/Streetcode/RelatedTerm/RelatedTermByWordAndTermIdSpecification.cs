using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelatedTermEntity = Streetcode.Domain.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.Specifications.Streetcode.RelatedTerm
{
    public class RelatedTermByWordAndTermIdSpecification : BaseSpecification<RelatedTermEntity>
    {
        public RelatedTermByWordAndTermIdSpecification(string word, int termId)
            : base(rt => rt.Word.ToLower().Equals(word.ToLower()) && rt.TermId == termId)
        {
        }
    }
}
