using AutoMapper;
using Streetcode.BLL.DTO.Team;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.BLL.Mapping.Team
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<Positions, PositionDto>().ReverseMap();
        }
    }
}
