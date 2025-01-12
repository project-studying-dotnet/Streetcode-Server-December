using AutoMapper;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.Domain.Entities.AdditionalContent;

namespace Streetcode.BLL.Mapping.AdditionalContent
{
    public class SubtitleProfile : Profile
    {
        public SubtitleProfile()
        {
            CreateMap<Subtitle, SubtitleDto>().ReverseMap();
        }
    }
}
