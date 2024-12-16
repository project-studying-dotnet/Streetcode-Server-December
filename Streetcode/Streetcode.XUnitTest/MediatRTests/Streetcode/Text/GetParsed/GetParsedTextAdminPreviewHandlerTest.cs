using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.GetParsed
{
    /// <summary>
    /// Unit tests for the GetParsedTextAdminPreviewHandler class, responsible for testing the parsing of text for admin preview.
    /// </summary>
    public class GetParsedTextAdminPreviewHandlerTest
    {
        private readonly Mock<ITextService> _textServiceMock;
        private readonly GetParsedTextAdminPreviewHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParsedTextAdminPreviewHandlerTest"/> class and sets up mocks for dependencies.
        /// </summary>
        public GetParsedTextAdminPreviewHandlerTest()
        {
            _textServiceMock = new Mock<ITextService>();
            _handler = new GetParsedTextAdminPreviewHandler(_textServiceMock.Object);
        }

        /// <summary>
        /// Verifies that the handler returns a successful result when the text is parsed correctly.
        /// </summary>
        /// <returns>A task representing the asynchronous unit test, which verifies the result is successful and contains the parsed text.</returns>
        [Fact]
        public async Task Handle_TextParsedSuccessfully_ReturnsOkResult()
        {
            // Arrange
            string textToParse = "Some text to parse";
            string parsedText = "Parsed text";
            var command = new GetParsedTextForAdminPreviewQuery(textToParse);

            _textServiceMock
                .Setup(service => service.AddTermsTag(textToParse))
                .ReturnsAsync(parsedText);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(parsedText);
            _textServiceMock.Verify(service => service.AddTermsTag(textToParse), Times.Once);
        }

        /// <summary>
        /// Verifies that the handler returns a failure result when text parsing fails.
        /// </summary>
        /// <returns>A task representing the asynchronous unit test, which verifies the result is failed and contains appropriate error messages.</returns>
        [Fact]
        public async Task Handle_TextParsingFailed_ReturnsFailResult()
        {
            // Arrange
            string textToParse = "Some text to parse";
            var command = new GetParsedTextForAdminPreviewQuery(textToParse);

            _textServiceMock.Setup(service => service.AddTermsTag(textToParse))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Contain("text was not parsed successfully");
            _textServiceMock.Verify(service => service.AddTermsTag(textToParse), Times.Once);
        }
    }
}
