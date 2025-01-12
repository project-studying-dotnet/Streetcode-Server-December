using AutoMapper;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.Domain.Entities.Analytics;

namespace Streetcode.BLL.Mapping.Analytics
{
    public class StatisticRecordProfile : Profile
    {
        public StatisticRecordProfile()
        {
            CreateMap<StatisticRecord, StatisticRecordDto>().ReverseMap();

            CreateMap<CreateStatisticRecordDto, StatisticRecord>();
        }
    }
}
