using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.AdminDeleteComment
{
    public record AdminDeleteCommentHandler : IRequestHandler<AdminDeleteCommentCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repository;

        public AdminDeleteCommentHandler(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<Result<Unit>> Handle(AdminDeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _repository.CommentRepository
                .GetFirstOrDefaultBySpecAsync(new CommentWithChildrenSpecification(request.Id));

            if (comment is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comment", request.Id);
                throw new Exception(errorMsg);
            }

            _repository.CommentRepository.Delete(comment);

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "comment", request.Id);
                throw new Exception(errorMsg);
            }

            return Result.Ok(Unit.Value);
        }
    }
}
