using EmailService.BLL.Consumers;
using EmailService.BLL.DTO.ConsumerDtos;
using EmailService.BLL.Interfaces;
using EmailService.BLL.Interfaces.Azure;
using EmailService.DAL.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmailService.XUnitTest.Consumers
{
    public class EmailMessageConsumerTest
    {
        [Fact]
        public async Task EmailMessageConsumer_ProcessesMessageSuccessfully()
        {
            // Arrange
            var mockServiceBus = new Mock<IAzureServiceBus>();
            var mockEmailService = new Mock<IEmailService>();
            var mockServiceScope = new Mock<IServiceScope>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            // Setup a test message
            var testEmailMessageDto = new EmailMessageConsumerDto
            {
                To = "test@example.com",
                From = "sender@example.com",
                Subject = "Test Subject",
                Content = "Test Content"
            };
            var testMessageJson = JsonConvert.SerializeObject(testEmailMessageDto);

            mockServiceBus
                .Setup(sb => sb.ReceiveMessage(It.IsAny<string>()))
                .ReturnsAsync(testMessageJson);

            mockEmailService
                .Setup(es => es.SendEmailAsync(It.IsAny<Message>()))
                .Returns(Task.FromResult(true));

            mockServiceScope
                .Setup(s => s.ServiceProvider)
                .Returns(new ServiceCollection()
                    .AddSingleton(mockEmailService.Object)
                    .BuildServiceProvider());

            mockServiceScopeFactory
                .Setup(f => f.CreateScope())
                .Returns(mockServiceScope.Object);

            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockServiceScopeFactory.Object);

            var emailMessageConsumer = new EmailMessageConsumer(mockServiceBus.Object, serviceProviderMock.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            // Act
            await emailMessageConsumer.StartAsync(cts.Token);

            // Assert
            mockServiceBus.Verify(sb => sb.ReceiveMessage("emailQueue"), Times.AtLeastOnce);
            mockEmailService.Verify(es => es.SendEmailAsync(It.IsAny<Message>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task EmailMessageConsumer_NoMessage_DoesNotCallSendEmail()
        {
            // Arrange
            var mockServiceBus = new Mock<IAzureServiceBus>();
            var mockEmailService = new Mock<IEmailService>();
            var mockServiceScope = new Mock<IServiceScope>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            mockServiceBus
                .Setup(sb => sb.ReceiveMessage(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IEmailService)))
                .Returns(mockEmailService.Object);

            var emailMessageConsumer = new EmailMessageConsumer(mockServiceBus.Object, serviceProviderMock.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            // Act
            await emailMessageConsumer.StartAsync(cts.Token);

            // Assert
            mockServiceBus.Verify(sb => sb.ReceiveMessage("emailQueue"), Times.AtLeastOnce);
            mockEmailService.Verify(es => es.SendEmailAsync(It.IsAny<Message>()), Times.Never);
        }
    }
}
