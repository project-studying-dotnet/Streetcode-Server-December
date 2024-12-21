using Streetcode.DAL.Specification;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.Specifications.Streetcode.Fact
{
    public class GetAllFactsByStreetcodeIdSpecification : BaseSpecification<FactEntity>
    {
        public GetAllFactsByStreetcodeIdSpecification(int streetcodeId) : base(f => f.StreetcodeId == streetcodeId)
        {
        }
    }
}
