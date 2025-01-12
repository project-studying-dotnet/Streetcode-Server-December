using AutoMapper;
using Streetcode.BLL.DTO.Sources;
using Streetcode.Domain.Entities.Sources;

namespace Streetcode.BLL.Mapping.Sources
{
    public class SourceLinkSubCategoryProfile : Profile
    {
        public SourceLinkSubCategoryProfile()
        {
            CreateMap<CategoryContentCreateDto, StreetcodeCategoryContent>().ReverseMap();
        }
    }
}
