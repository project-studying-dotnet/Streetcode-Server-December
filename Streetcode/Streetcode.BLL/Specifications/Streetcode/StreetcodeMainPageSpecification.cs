using Streetcode.DAL.Specification;
using Streetcode.Domain.Entities.Streetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Specifications.Streetcode
{
    public class StreetcodeMainPageSpecification : BaseSpecification<StreetcodeContent>
    {
        public StreetcodeMainPageSpecification(int id)
            : base(rt => rt.Id == id)
        {
            AddInclude(x => x.Tags);
            AddInclude(x => x.Audio!);
            AddInclude(x => x.Images);
        }
    }
}
