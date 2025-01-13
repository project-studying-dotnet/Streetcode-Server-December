using EmailService.BLL.Interfaces;
using Serilog;
using Serilog.Events;

namespace EmailService.BLL.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

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
           Log(LogEventLevel.Information, "{Message}", msg);
        }

        public void LogWarning(string msg)
        {
			Log(LogEventLevel.Warning, "{Message}", msg);
		}

        public void LogTrace(string msg)
        {
            Log(LogEventLevel.Information, "{Message}", msg);
        }

        public void LogDebug(string msg)
        {
            Log(LogEventLevel.Debug, "{Message}", msg);
        }

        public void LogError(object request, string errorMsg)
        {
            string requestType = request.GetType().ToString();
            string requestClass = requestType.Substring(requestType.LastIndexOf('.') + 1);
			Log(LogEventLevel.Error, "{RequestClass} handled with the error: {ErrorMessage}", requestClass, errorMsg);
		}
    }
}
