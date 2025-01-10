using EmailService.BLL.Interfaces;
using FluentResults;
using MediatR;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Mediatr.Email
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result<Unit>>
    {
        private readonly ILoggerService _logger;
        private readonly ICacheService _cacheService;
        //private readonly IUserMicroserviceClient _userMicroserviceClient;

        public ConfirmEmailHandler(ICacheService cacheService, /*IUserMicroserviceClient userMicroserviceClient*/ ILoggerService logger)
        {
            _cacheService = cacheService;
            //_userMicroserviceClient = userMicroserviceClient;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _cacheService.GetAsync(request.Email);

            if (storedToken == null || storedToken != request.Token)
            {
                const string errorMsg = $"Invalid or expired token";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            // confirem email in other microservice
            //await _userMicroserviceClient.ConfirmUserEmailAsync(request.Email);

            return Result.Ok(Unit.Value);
        }
    }
}
