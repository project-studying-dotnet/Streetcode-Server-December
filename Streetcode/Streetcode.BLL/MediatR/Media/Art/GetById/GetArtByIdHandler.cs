﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Art.GetById
{
    public class GetArtByIdHandler : IRequestHandler<GetArtByIdQuery, Result<ArtDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetArtByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ArtDto>> Handle(GetArtByIdQuery request, CancellationToken cancellationToken)
        {
            var art = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);

            if (art is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "art", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<ArtDto>(art));
        }
    }
}