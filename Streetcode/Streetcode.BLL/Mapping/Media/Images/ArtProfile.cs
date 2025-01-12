using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images
{
    public class ArtProfile : Profile
    {
        public ArtProfile()
        {
            CreateMap<Art, ArtDto>().ReverseMap();
            CreateMap<ArtCreateDto, Art>();
        }
    }
}
