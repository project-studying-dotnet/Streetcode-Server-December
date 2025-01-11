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
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly ICacheService _cacheService;

        public ConfirmEmailHandler(ICacheService cacheService, ILoggerService logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _cacheService.GetAsync(request.Email);

            if (storedToken == null || storedToken != request.Token)
            {
                const string errorMsg = $"Invalid or expired token";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok("Email confirmed");
        }
    }
}
