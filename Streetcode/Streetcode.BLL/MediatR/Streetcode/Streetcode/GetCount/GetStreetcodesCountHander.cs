﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllShort;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetCount
{
    public class GetStreetcodesCountHander : IRequestHandler<GetStreetcodesCountQuery,
        Result<int>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetStreetcodesCountHander(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(GetStreetcodesCountQuery request, CancellationToken cancellationToken)
        {
            var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync();

            if (streetcodes != null)
            {
                return Result.Ok(streetcodes.Count());
            }

            string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "streetcode");
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
