using EmailService.BLL.Interfaces;
using FluentResults;
using MediatR;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace EmailService.BLL.Mediatr.Email
{
    public class SendEmailWithVerificationHandler : IRequestHandler<SendEmailWithVerificationCommand, Result<Unit>>
    {
        private readonly ILoggerService _logger;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;
        //private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;


        public SendEmailWithVerificationHandler(IEmailService emailService, ICacheService cacheService, ILoggerService logger, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _emailService = emailService;
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
            //_urlHelper = urlHelper;
            _linkGenerator = linkGenerator;
        }

        public async Task<Result<Unit>> Handle(SendEmailWithVerificationCommand request, CancellationToken cancellationToken)
        {
            // generate token
            var token = Guid.NewGuid().ToString();
            var expirationTime = DateTime.UtcNow.AddMinutes(2); // expire in 2 min

            // save token to redis cahe
            await _cacheService.SetAsync(request.Email, token, TimeSpan.FromMinutes(2));

            // link on our domain
            //var httpContext = new DefaultHttpContext();
            //var urlHelper = new UrlHelper(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()));

            //var confirmationLink = urlHelper.Action("ConfirmEmail", "Email", new { email = request.Email, token = token }, "https");

            var confirmationLink = _linkGenerator.GetUriByAction(
               httpContext: _httpContextAccessor.HttpContext,
               action: "ConfirmEmail",
               controller: "Email",
               values: new { email = request.Email, token = token }
           );

            var isResultSuccess = await _emailService.SendConfirmationEmailAsync(request.Email, confirmationLink);

            if (isResultSuccess)
            {
                return Result.Ok(Unit.Value);
            }
            else
            {
                const string errorMsg = $"Failed to send email with verification";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
