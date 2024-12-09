using MediatR;
using Moq;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.EmailTests
{
    public class SendEmailHandlerTest
    {
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ILoggerService> _loggerServiceMock;
        private SendEmailHandler _sendEmailHandler;

        public SendEmailHandlerTest()
        {
            _emailServiceMock = new();
            _loggerServiceMock = new();
            _sendEmailHandler = new(_emailServiceMock.Object, _loggerServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccsesfullResult_WhenEmailSentSuccsesfully()
        {
            //A(Arrange):

            var emailCommand = new SendEmailCommand(new EmailDTO { From = "Me", Content = "email sending succsesfully test!" });
            _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>())).ReturnsAsync(true);

            //A(Act):

            var res = await _sendEmailHandler.Handle(emailCommand , CancellationToken.None);

            //A(Assert):

            Assert.True(res.IsSuccess);
            Assert.Equal(Unit.Value, res.Value);

            _emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
            _loggerServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenEmailNotSent()
        {
            //A(Arrange):

            var emailCommand = new SendEmailCommand(new EmailDTO { From = "Me", Content = "email sending fail test!" });
            _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>())).ReturnsAsync(false);
            _loggerServiceMock.Setup(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()));

            //A(Act):

            var res = await _sendEmailHandler.Handle(emailCommand, CancellationToken.None);

            //A(Assert):

            Assert.False(res.IsSuccess);
            Assert.Equal("Failed to send email message", res.Errors[0].Message);

            _emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
            _loggerServiceMock.Verify(l => l.LogError(emailCommand, "Failed to send email message"), Times.Once);
        }
    }
}
