using Streetcode.DAL.Specification;
using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.BLL.Specifications.Media.Audio
{
    public class GetAllAudioSpecification : BaseSpecification<AudioEntity>
    {
        public GetAllAudioSpecification()
            : base(null) // No filtering condition
        {
        }
    }
}