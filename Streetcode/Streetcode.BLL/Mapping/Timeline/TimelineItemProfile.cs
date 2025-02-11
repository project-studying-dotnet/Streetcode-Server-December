using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.DTO.Timeline.Update;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline
{
    public class TimelineItemProfile : Profile
    {
        public TimelineItemProfile()
        {
            CreateMap<TimelineItem, TimelineItemDto>().ReverseMap();
            CreateMap<TimelineItem, TimelineItemCreateDto>().ReverseMap();
            CreateMap<TimelineItem, TimelineItemUpdateDto>().ReverseMap();
            CreateMap<TimelineItem, TimelineItemDto>()
                .ForMember(dest => dest.HistoricalContexts, opt => opt.MapFrom(x => x.HistoricalContextTimelines
                    .Select(x => new HistoricalContextDto
                    {
                        Id = x.HistoricalContextId,
                        Title = x.HistoricalContext.Title
                    }).ToList()));
        }
    }
}
