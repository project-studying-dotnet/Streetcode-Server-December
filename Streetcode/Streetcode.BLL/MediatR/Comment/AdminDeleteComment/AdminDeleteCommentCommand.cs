using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.AdminDeleteComment
{
    public record AdminDeleteCommentCommand(int Id) : IRequest<Result<Unit>>;
}
