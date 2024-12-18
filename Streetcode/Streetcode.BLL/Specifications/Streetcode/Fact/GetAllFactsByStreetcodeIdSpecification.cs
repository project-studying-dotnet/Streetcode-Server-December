using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
