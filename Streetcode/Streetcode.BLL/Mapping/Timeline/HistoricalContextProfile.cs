using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.Domain.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline
{
    public class HistoricalContextProfile : Profile
    {
        public HistoricalContextProfile()
        {
            CreateMap<HistoricalContext, HistoricalContextDto>().ReverseMap();
        }
    }
}
