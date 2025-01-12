using AutoMapper;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.Domain.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Image, ImageDto>().ReverseMap();

            CreateMap<ImageFileBaseCreateDto, Image>();
        }
    }
}
