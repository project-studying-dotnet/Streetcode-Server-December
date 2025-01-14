using EmailService.BLL.Interfaces;
using Serilog;
using Serilog.Events;

namespace EmailService.BLL.Services
{
   public class LoggerService : ILoggerService
   {
        private readonly ILogger _logger;
        private const string MessageTemplate = "{Message}";
        private const string ErrorMessageTemplate = "{RequestClass} handled with the error: {ErrorMessage}";

        public LoggerService(ILogger logger)
        {
            _logger = logger;
        }

        private void Log(LogEventLevel level, string message, params object[] values)
        {
            _logger.Write(level, message, values);
        }

        public void LogInformation(string msg)
        {
            Log(LogEventLevel.Information, MessageTemplate, msg);
        }

        public void LogWarning(string msg)
        {
            Log(LogEventLevel.Warning, MessageTemplate, msg);
        }

        public void LogTrace(string msg)
        {
            Log(LogEventLevel.Information, MessageTemplate, msg);
        }

        public void LogDebug(string msg)
        {
            Log(LogEventLevel.Debug, MessageTemplate, msg);
        }

        public void LogError(object request, string errorMsg)
        {
            string requestType = request.GetType().ToString();
            string requestClass = requestType.Substring(requestType.LastIndexOf('.') + 1);
            Log(LogEventLevel.Error, ErrorMessageTemplate, requestClass, errorMsg);
        }
   }
}
