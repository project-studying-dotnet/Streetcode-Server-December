using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.GetCommentByIdWithReplies
{
    public record GetCommentByIdWithRepliesQuery(int Id) : IRequest<Result<GetCommentDto>>
    {
    }
}
