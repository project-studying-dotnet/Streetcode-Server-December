using AutoMapper;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.BLL.Mapping.AdditionalContent
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>().ForMember(x => x.Streetcodes, conf => conf.Ignore());
            CreateMap<Tag, StreetcodeTagDto>().ReverseMap();
            CreateMap<StreetcodeTagIndex, StreetcodeTagDto>()
                .ForMember(x => x.Id, conf => conf.MapFrom(ti => ti.TagId))
                .ForMember(x => x.Title, conf => conf.MapFrom(ti => ti.Tag.Title ?? ""));
        }
    }
}
