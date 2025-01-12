using AutoMapper;
using Streetcode.BLL.DTO.Sources;
using Streetcode.Domain.Entities.Sources;

namespace Streetcode.BLL.Mapping.Sources
{
    public class StreetcodeCategoryContentProfile : Profile
    {
        public StreetcodeCategoryContentProfile()
        {
            CreateMap<StreetcodeCategoryContent, StreetcodeCategoryContentDto>()
                .ReverseMap();
        }
    }
}
