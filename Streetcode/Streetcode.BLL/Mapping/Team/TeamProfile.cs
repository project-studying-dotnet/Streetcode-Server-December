using AutoMapper;
using Streetcode.BLL.DTO.Team;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.BLL.Mapping.Team
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<TeamMember, TeamMemberDto>().ReverseMap();
        }
    }
}
