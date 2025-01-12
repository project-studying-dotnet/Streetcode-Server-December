using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Streetcode.Types;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Streetcode.Types;

namespace Streetcode.BLL.Mapping.Streetcode.Types
{
    public class EventStreetcodeProfile : Profile
    {
        public EventStreetcodeProfile()
        {
            CreateMap<EventStreetcode, EventStreetcodeDto>()
                .IncludeBase<StreetcodeContent, StreetcodeDto>().ReverseMap();
            CreateMap<StreetcodeMainPageCreateDto, EventStreetcode>();
        }
    }
}
