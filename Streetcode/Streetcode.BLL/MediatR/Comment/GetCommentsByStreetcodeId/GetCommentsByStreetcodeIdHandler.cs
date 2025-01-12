using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId
{
    public record GetCommentsByStreetcodeIdHandler : IRequestHandler<GetCommentsByStreetcodeIdQuery, Result<IEnumerable<GetCommentDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public GetCommentsByStreetcodeIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<GetCommentDto>>> Handle(GetCommentsByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repository.CommentRepository
                .GetAllBySpecAsync(new CommentByStreetcodeIdSpecification(request.id));

            if (comments is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "comment", request.id);
                _logger.LogError(request, errorMsg);
                return new Error(errorMsg);
            }

            var commentsDtos = _mapper.Map<IEnumerable<GetCommentDto>>(comments);

            if (commentsDtos is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "DTOS for comments");
                _logger.LogError(request, errorMsg);
                return new Error(errorMsg);
            }

            return Result.Ok(commentsDtos);
        }
    }
}
