using Streetcode.DAL.Specification;
using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.BLL.Specifications.Media.Audio
{
    public class GetAudioByIdSpecification : BaseSpecification<AudioEntity>
    {
        public GetAudioByIdSpecification(int id)
            : base(audio => audio.Id == id)
        {
        }
    }
}