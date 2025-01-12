using AutoMapper;
using Streetcode.BLL.DTO.Team;
using Streetcode.Domain.Entities.Team;

namespace Streetcode.BLL.Mapping.Team
{
    public class TeamLinkProfile : Profile
    {
        public TeamLinkProfile()
        {
            CreateMap<TeamMemberLink, TeamMemberLinkDto>().ReverseMap();
        }
    }
}
