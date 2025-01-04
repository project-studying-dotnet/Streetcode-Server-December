using EmailService.BLL.DTO;
using EmailService.BLL.Interfaces;
using EmailService.BLL.Mediatr.Email;
using EmailService.DAL.Entities;
using MediatR;
using Moq;
using Xunit;

namespace EmailService.XUnitTest
{
    public class EmailTests
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ILoggerService> _loggerServiceMock;
        private SendEmailHandler _sendEmailHandler;

        public EmailTests()
        {
            _emailServiceMock = new();
            _loggerServiceMock = new();
            _sendEmailHandler = new(_emailServiceMock.Object, _loggerServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccsesfullResult_WhenEmailSentSuccsesfully()
        {
            // A(Arrange):

            var emailCommand = new SendEmailCommand(new EmailDto { ToEmail = new List<string> { "email1@gmail.com", "email_@gmail.com" }, FromText = "Me", Content = "email sending succsesfully test!" });
            _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>())).ReturnsAsync(true);

            // A(Act):

            var res = await _sendEmailHandler.Handle(emailCommand, CancellationToken.None);

            // A(Assert):

            Xunit.Assert.True(res.IsSuccess);
            Xunit.Assert.Equal(Unit.Value, res.Value);

            _emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
            _loggerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenEmailNotSent()
        {
            // A(Arrange):

            var emailCommand = new SendEmailCommand(new EmailDto {ToEmail = new List<string> {"email1@gmail.com", "email2@gmail.com" } , FromText = "Me", Content = "email sending fail test!" });
            _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>())).ReturnsAsync(false);
            _loggerServiceMock.Setup(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()));

            // A(Act):

            var res = await _sendEmailHandler.Handle(emailCommand, CancellationToken.None);

            // A(Assert):

            Xunit.Assert.False(res.IsSuccess);
            Xunit.Assert.Equal("Failed to send email message", res.Errors[0].Message);

            _emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
            _loggerServiceMock.Verify(l => l.LogError(emailCommand, "Failed to send email message"), Times.Once);
        }

        //TO CONTINUE DUE TO NEW SCENARIOS
    }
}
