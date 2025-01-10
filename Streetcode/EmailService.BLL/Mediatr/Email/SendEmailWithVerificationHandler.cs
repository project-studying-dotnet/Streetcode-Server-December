using EmailService.BLL.Interfaces;
using FluentResults;
using MediatR;

namespace EmailService.BLL.Mediatr.Email
{
    public class SendEmailWithVerificationHandler : IRequestHandler<SendEmailWithVerificationCommand, Result<Unit>>
    {
        private readonly ILoggerService _logger;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public SendEmailWithVerificationHandler(IEmailService emailService, ICacheService cacheService, ILoggerService logger)
        {
            _logger = logger;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        public async Task<Result<Unit>> Handle(SendEmailWithVerificationCommand request, CancellationToken cancellationToken)
        {
            // generate token
            var token = Guid.NewGuid().ToString();
            var expirationTime = DateTime.UtcNow.AddMinutes(2); // expire in 2 min

            // save token to redis cahe
            await _cacheService.SetAsync(request.Email, token, TimeSpan.FromMinutes(2));

            // link on our domain
            var confirmationLink = $"https://yourdomain.com/api/email/confirm?email={request.Email}&token={token}";

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
