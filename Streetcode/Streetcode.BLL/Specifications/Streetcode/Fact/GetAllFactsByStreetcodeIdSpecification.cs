using FactEntity = Streetcode.Domain.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.Specifications.Streetcode.Fact
{
    public class GetAllFactsByStreetcodeIdSpecification : BaseSpecification<FactEntity>
    {
        public GetAllFactsByStreetcodeIdSpecification(int streetcodeId) : base(f => f.StreetcodeId == streetcodeId)
        {
        }
    }
}
