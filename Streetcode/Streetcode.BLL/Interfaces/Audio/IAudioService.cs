using Streetcode.BLL.DTO.Media.Audio;

namespace Streetcode.BLL.Interfaces.Audio
{
    public interface IAudioService
    {
        public DAL.Entities.Media.Audio ConfigureAudio(AudioFileBaseCreateDto audioDTO);
    }
}
