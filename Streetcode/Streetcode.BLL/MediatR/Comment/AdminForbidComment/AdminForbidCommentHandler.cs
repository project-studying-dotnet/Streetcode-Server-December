using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.AdminForbidComment
{
    public record AdminForbidCommentHandler : IRequestHandler<AdminForbidCommentCommand, Result<string>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public AdminForbidCommentHandler(IRepositoryWrapper repository , ILoggerService logger)
        {
            _repository = repository;
        }

        public async Task<Result<string>> Handle(AdminForbidCommentCommand request, CancellationToken cancellationToken)
        {
            var allInReviewComments = await _repository.CommentRepository
                .GetAllAsync(c => c.Status == CommentStatus.InReview);

            if (allInReviewComments is null)
            {
                var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "comment with 'InReview' status", request.Id);
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }

            var commentToDelete = allInReviewComments.FirstOrDefault(c => c.Id == request.Id);

            if (commentToDelete != null)
            {
                commentToDelete.Status = CommentStatus.Prohibited;
                _repository.CommentRepository.Update(commentToDelete);
                await _repository.SaveChangesAsync();
            }

            return Result.Ok("Succsesfully changed status for comment with forbidden content. It will be not shown in comments!");
        }
    }
}
