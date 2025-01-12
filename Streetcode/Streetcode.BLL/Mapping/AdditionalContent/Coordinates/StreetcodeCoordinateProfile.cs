using AutoMapper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.Mapping.AdditionalContent.Coordinates
{
    public class StreetcodeCoordinateProfile : Profile
    {
        public StreetcodeCoordinateProfile()
        {
            CreateMap<StreetcodeCoordinate, StreetcodeCoordinateDto>().ReverseMap();
        }
    }
}