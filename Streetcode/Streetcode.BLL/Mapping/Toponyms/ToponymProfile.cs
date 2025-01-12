using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Toponyms;

namespace Streetcode.BLL.Mapping.Toponyms
{
    public class ToponymProfile : Profile
    {
        public ToponymProfile()
        {
            CreateMap<Toponym, ToponymDto>().ReverseMap();
            CreateMap<StreetcodeContent, StreetcodeDto>();
        }
    }
}
