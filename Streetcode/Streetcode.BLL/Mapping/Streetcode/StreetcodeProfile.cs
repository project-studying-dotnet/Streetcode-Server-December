using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Mapping.Streetcode
{
    public class StreetcodeProfile : Profile
    {
        public StreetcodeProfile()
        {
            CreateMap<StreetcodeContent, StreetcodeDto>()
                .ForMember(x => x.StreetcodeType, conf => conf.MapFrom(s => GetStreetcodeType(s)))
                .ReverseMap();
            CreateMap<StreetcodeContent, StreetcodeShortDto>().ReverseMap();
            CreateMap<StreetcodeContent, StreetcodeMainPageDto>()
                 .ForPath(dto => dto.Text, conf => conf
                    .MapFrom(e => e.Text.Title))
                .ForPath(dto => dto.ImageId, conf => conf
                    .MapFrom(e => e.Images.Select(i => i.Id).LastOrDefault()));
        }

        private StreetcodeType GetStreetcodeType(StreetcodeContent streetcode)
        {
            if (streetcode is EventStreetcode)
            {
                return StreetcodeType.Event;
            }

            return StreetcodeType.Person;
        }
    }
}
