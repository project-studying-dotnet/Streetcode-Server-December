using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.Specifications.Streetcode.RelatedTerm
{
    public class RelatedTermByWordSpecification : BaseSpecification<RelatedTermEntity>
    {
        public RelatedTermByWordSpecification(string word)
            : base(rt => rt.Word.ToLower().Equals(word.ToLower()))
        {
        }
    }
}
