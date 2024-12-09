using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll
{
    public record GetAllRelatedTermsQuery : IRequest<Result<IEnumerable<RelatedTermDTO>>>
    {
    }
}
