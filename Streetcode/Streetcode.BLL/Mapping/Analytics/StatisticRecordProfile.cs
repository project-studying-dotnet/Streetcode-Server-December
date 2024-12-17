using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.News;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.News;

namespace Streetcode.BLL.Mapping.Analytics
{
    public class StatisticRecordProfile : Profile
    {
        public StatisticRecordProfile()
        {
            CreateMap<StatisticRecord, StatisticRecordDTO>().ReverseMap();

            CreateMap<CreateStatisticRecordDTO, StatisticRecord>();
        }
    }
}
