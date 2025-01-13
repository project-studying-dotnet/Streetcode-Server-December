using AutoMapper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;

namespace Streetcode.BLL.Services.Audio
{
    public class AudioService : IAudioService
    {
        private readonly IMapper _mapper;
        private readonly IBlobService _blobService;

        public AudioService(IMapper mapper, IBlobService blobService)
        {
            _mapper = mapper;
            _blobService = blobService;
        }

        public DAL.Entities.Media.Audio ConfigureAudio(AudioFileBaseCreateDto audioDTO)
        {
            string hashBlobStorageName = _blobService.SaveFileInStorage(
                audioDTO.BaseFormat,
                audioDTO.Title,
                audioDTO.Extension).Result;

            var audio = _mapper.Map<DAL.Entities.Media.Audio>(audioDTO);

            audio.BlobName = $"{hashBlobStorageName}.{audioDTO.Extension}";

            return audio;
        }

        public void DeleteAudio(string blobName)
        {
            _blobService.DeleteFileInStorage(blobName);
        }
    }
}
