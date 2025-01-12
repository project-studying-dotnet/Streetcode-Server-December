using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Streetcode.Types;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Streetcode.Types;

namespace Streetcode.BLL.Mapping.Streetcode.Types
{
    public class PersonStreetcodeProfile : Profile
    {
        public PersonStreetcodeProfile()
        {
            CreateMap<PersonStreetcode, PersonStreetcodeDto>()
                .IncludeBase<StreetcodeContent, StreetcodeDto>().ReverseMap();
            CreateMap<StreetcodeMainPageCreateDto, PersonStreetcode>();
        }
    }
}
