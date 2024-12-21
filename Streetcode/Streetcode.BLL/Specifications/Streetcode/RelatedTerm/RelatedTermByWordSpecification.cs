using Streetcode.DAL.Specification;
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
