using AutoMapper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.Domain.Entities.Media.Images;
using Streetcode.Domain.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent
{
    public class FactProfile : Profile
    {
        public FactProfile()
        {
            CreateMap<Fact, FactDto>().ReverseMap();
            CreateMap<Fact, CreateFactDto>().ReverseMap();
            CreateMap<Image, CreateFactImageDto>().ReverseMap();
            CreateMap<ImageDetails, CreateFactImageDetailsDto>().ReverseMap();
            CreateMap<Fact, FactUpdateCreateDto>().ReverseMap();
        }
    }
}
