using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.MediatR.Analytics.Delete;
using Streetcode.BLL.MediatR.Analytics;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.MediatR.Comment.GetCommentsToReview;

namespace Streetcode.WebApi.Controllers.Comment
{
    public class CommentController : BaseApiController
    {
        [HttpGet("{streetcodeid:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeid)
        {
            return HandleResult(await Mediator.Send(new GetCommentsByStreetcodeIdQuery(streetcodeid)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllToReview([FromQuery] List<string> restrictedWords)
        {
            return HandleResult(await Mediator.Send(new GetCommentsToReviewQuery(restrictedWords)));
        }
    }
}
