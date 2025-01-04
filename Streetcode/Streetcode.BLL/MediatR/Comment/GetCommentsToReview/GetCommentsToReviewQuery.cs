using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comment.GetCommentsToReview
{
    public record GetCommentsToReviewQuery(List<string> restrictedWords) : IRequest<Result<IEnumerable<GetCommentsToReviewDto>>>
    {
    }
}
