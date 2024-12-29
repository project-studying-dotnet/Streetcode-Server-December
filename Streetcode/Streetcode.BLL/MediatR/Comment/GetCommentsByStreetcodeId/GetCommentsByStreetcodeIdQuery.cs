using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId
{
    public record GetCommentsByStreetcodeIdQuery(int id) : IRequest<Result<IEnumerable<GetCommentDto>>>
    {
    }
}
