using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.BLL.Mapping.Media.Images
{
    public class StreetcodeArtProfile : Profile
    {
        public StreetcodeArtProfile()
        {
            CreateMap<StreetcodeArt, StreetcodeArtDto>().ReverseMap();
        }
    }
}
