using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Analytics.Delete;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.BLL.MediatR.Comment.CreateComment;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.MediatR.Comment.GetCommentsToReview;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;

namespace Streetcode.WebApi.Controllers.Comment
{
    [Route("[action]")]
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

        [AuthorizeRoles(UserRole.Admin)]
        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> AdminDeleteComment([FromRoute] int Id)
        {
            return HandleResult(await Mediator.Send(new AdminDeleteCommentCommand(Id)));
        }

        [HttpPost]
        public async Task<ActionResult<GetCommentDto>> Create([FromBody] CreateCommentDto createCommentDto)
        {
            return HandleResult(await Mediator.Send(new CreateCommentCommand(createCommentDto)));
        }
    }
}
