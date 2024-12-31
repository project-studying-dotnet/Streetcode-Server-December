using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.BLL.MediatR.Comment.GetCommentByIdWithReplies
{
    public class GetCommentByIdWithRepliesHandler : IRequestHandler<GetCommentByIdWithRepliesQuery, Result<GetCommentDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public GetCommentByIdWithRepliesHandler(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<GetCommentDto>> Handle(GetCommentByIdWithRepliesQuery request, CancellationToken cancellationToken)
        {
            var comment = await _repository.CommentRepository.GetFirstOrDefaultBySpecAsync(new CommentWithChildrenSpecification(request.Id));

            if (comment == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comment", request.Id);
                throw new Exception(errorMsg);
            }

            var commentDto = _mapper.Map<GetCommentDto>(comment);

            if (commentDto == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("ConvertationError", "comment", "commentDto");
                throw new Exception(errorMsg);
            }

            return Result.Ok(commentDto);
        }
    }
}
