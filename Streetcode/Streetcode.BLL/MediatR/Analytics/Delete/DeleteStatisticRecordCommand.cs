using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Analytics.Delete
{
    public record DeleteStatisticRecordCommand(int id) : IRequest<Result<bool>>;
}