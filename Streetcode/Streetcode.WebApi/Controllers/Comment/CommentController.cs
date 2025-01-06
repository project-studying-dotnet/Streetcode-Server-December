using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Analytics.Delete;
using Streetcode.BLL.MediatR.Analytics;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.MediatR.Comment.GetCommentsToReview;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using Streetcode.BLL.MediatR.Comment.UpdateComment;
using Streetcode.BLL.MediatR.Comment.GetCommentByIdWithReplies;
using Streetcode.BLL.MediatR.Comment.CreateComment;
using Streetcode.BLL.MediatR.Comment.UserDeleteComment;
using Streetcode.DAL.Enums;
using Streetcode.BLL.MediatR.Comment.GetCommentByStatus;

namespace Streetcode.WebApi.Controllers.Comment
{
    [Route("[action]")]
    public class CommentController : BaseApiController
    {
        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            return HandleResult(await Mediator.Send(new GetCommentByIdWithRepliesQuery(Id)));
        }

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

        [HttpPost]
        public async Task<ActionResult<GetCommentDto>> Create([FromBody] CreateCommentDto createCommentDto)
        {
            return HandleResult(await Mediator.Send(new CreateCommentCommand(createCommentDto)));
        }

        [HttpPut]
        public async Task<ActionResult<GetCommentDto>> Update([FromBody] UpdateCommentDto updateCommentDto)
        {
            return HandleResult(await Mediator.Send(new UpdateCommentCommand(updateCommentDto)));
        }
        
        [AuthorizeRoles(UserService.DAL.Enums.UserRole.Admin)]
        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> AdminDeleteComment([FromRoute] int Id)
        {
            return HandleResult(await Mediator.Send(new AdminDeleteCommentCommand(Id)));
        }
        
        [HttpDelete]
        public async Task<IActionResult> UserDeleteComment([FromBody] UserDeleteCommentDto userDeleteCommentDto)
        {
            return HandleResult(await Mediator.Send(new UserDeleteCommentCommand(userDeleteCommentDto)));
        }

        // a

        [HttpGet]
        public async Task<ActionResult<List<GetCommentDto>>> GetByStatus([FromQuery] CommentStatus status)
        {
            return HandleResult(await Mediator.Send(new GetCommentByStatusCommand(status)));
        }
    }
}
