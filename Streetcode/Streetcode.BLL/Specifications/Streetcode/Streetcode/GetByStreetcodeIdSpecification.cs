using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Specifications.Streetcode.Streetcode
{
    public class GetByStreetcodeIdSpecification : BaseSpecification<StreetcodeContent>
    {
        public GetByStreetcodeIdSpecification(int streetcodeId)
            : base(id => id.Id == streetcodeId)
        {
        }
    }
}