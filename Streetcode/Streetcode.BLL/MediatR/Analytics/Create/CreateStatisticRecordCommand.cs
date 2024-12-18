using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.News;

namespace Streetcode.BLL.MediatR.Analytics
{
    public record CreateStatisticRecordCommand(CreateStatisticRecordDTO createStatisticRecord)
        : IRequest<Result<StatisticRecordDTO>>;
}
