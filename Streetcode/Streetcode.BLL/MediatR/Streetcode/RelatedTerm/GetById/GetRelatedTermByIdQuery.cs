using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById
{
    public record GetRelatedTermByIdQuery(int id) : IRequest<Result<RelatedTermDto>>
    {
    }
}
