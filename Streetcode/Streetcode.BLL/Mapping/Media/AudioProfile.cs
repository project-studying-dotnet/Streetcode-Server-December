using AutoMapper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.DAL.Entities.Media;

namespace Streetcode.BLL.Mapping.Media
{
    public class AudioProfile : Profile
    {
        public AudioProfile()
        {
            CreateMap<Audio, AudioDto>()
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));

            CreateMap<AudioFileBaseCreateDto, Audio>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));
        }
    }
}