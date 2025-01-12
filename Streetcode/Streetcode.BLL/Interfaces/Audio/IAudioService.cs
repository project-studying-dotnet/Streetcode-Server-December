using Streetcode.BLL.DTO.Media.Audio;

namespace Streetcode.BLL.Interfaces.Audio
{
    public interface IAudioService
    {
        public Domain.Entities.Media.Audio ConfigureAudio(AudioFileBaseCreateDto audioDTO);
        public void DeleteAudio(string blobName);
    }
}
