using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.MediatR.Analytics.Delete;
using Streetcode.BLL.MediatR.Analytics;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;

namespace Streetcode.WebApi.Controllers.Comment
{
    public class CommentController : BaseApiController
    {
        [HttpGet("{streetcodeid:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeid)
        {
            return HandleResult(await Mediator.Send(new GetCommentsByStreetcodeIdQuery(streetcodeid)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> AdminDeleteComment([FromRoute] int Id)
        {
            return HandleResult(await Mediator.Send(new AdminDeleteCommentCommand(Id)));
        }
    }
}
