using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Specifications.Streetcode.Streetcode
{
    public class GetStreetcodeWithAudioSpecification : BaseSpecification<StreetcodeContent>
    {
        public GetStreetcodeWithAudioSpecification(int streetcodeId)
            : base(streetcode => streetcode.Id == streetcodeId)
        {
            AddInclude(streetcode => streetcode.Audio!); 
        }
    }
}
