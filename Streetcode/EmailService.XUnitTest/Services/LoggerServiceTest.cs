using EmailService.BLL.Services;
using Moq;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Xunit;

namespace EmailService.XUnitTest.Services
{
	public class LoggerServiceTests
	{
		private readonly Mock<ILogger> _loggerMock;
		private readonly LoggerService _loggerService;

		public LoggerServiceTests()
		{
			_loggerMock = new Mock<ILogger>();
			_loggerService = new LoggerService(_loggerMock.Object);
		}

		[Fact]
		public void Log_CallsWriteWithCorrectParameters()
		{
			// Arrange
			var level = LogEventLevel.Information;
			var message = "Test log message";
			var values = new object[] { "value1", "value2" };

			// Act
			_loggerService.GetType().GetMethod("Log", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				.Invoke(_loggerService, new object[] { level, message, values });

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				level,
				message,
				It.Is<object[]>(args => args.Length == 2 && args[0].ToString() == "value1" && args[1].ToString() == "value2")),
				Times.Once);
		}


		[Fact]
		public void LogInformation_CallsInformationWithCorrectMessage()
		{
			// Arrange
			var message = "Test information message";

			// Act
			_loggerService.LogInformation(message);

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				LogEventLevel.Information,
				"{Message}",
				It.Is<object[]>(args => args.Length == 1 && args[0].ToString() == message)),
				Times.Once);
		}

		[Fact]
		public void LogWarning_CallsWarningWithCorrectMessage()
		{
			// Arrange
			var message = "Test warning message";

			// Act
			_loggerService.LogWarning(message);

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				LogEventLevel.Warning,
				"{Message}",
				It.Is<object[]>(args => args.Length == 1 && args[0].ToString() == message)),
				Times.Once);
		}

		[Fact]
		public void LogTrace_CallsInformationWithCorrectMessage()
		{
			// Arrange
			var message = "Test trace message";

			// Act
			_loggerService.LogTrace(message);

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				LogEventLevel.Information,
				"{Message}",
				It.Is<object[]>(args => args.Length == 1 && args[0].ToString() == message)),
				Times.Once);
		}

		[Fact]
		public void LogDebug_CallsDebugWithCorrectMessage()
		{
			// Arrange
			var message = "Test debug message";

			// Act
			_loggerService.LogDebug(message);

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				LogEventLevel.Debug,
				"{Message}",
				It.Is<object[]>(args => args.Length == 1 && args[0].ToString() == message)),
				Times.Once);
		}

		[Fact]
		public void LogError_CallsErrorWithCorrectRequestTypeAndMessage()
		{
			// Arrange
			var request = new { Id = 1, Name = "TestRequest" };
			var errorMsg = "Test error message";
			var requestType = request.GetType().ToString();
			var requestClass = requestType.Substring(requestType.LastIndexOf('.') + 1);

			// Act
			_loggerService.LogError(request, errorMsg);

			// Assert
			_loggerMock.Verify(logger => logger.Write(
				LogEventLevel.Error,
				"{RequestClass} handled with the error: {ErrorMessage}",
				It.Is<object[]>(args =>
					args.Length == 2 &&
					args[0].ToString() == requestClass &&
					args[1].ToString() == errorMsg)),
				Times.Once);
		}
	}
}
