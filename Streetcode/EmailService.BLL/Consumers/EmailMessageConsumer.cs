using EmailService.BLL.DTO.ConsumerDtos;
using EmailService.BLL.Interfaces;
using EmailService.BLL.Interfaces.Azure;
using EmailService.DAL.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Consumers
{
    public class EmailMessageConsumer : BackgroundService
    {
        private readonly IAzureServiceBus _serviceBus;
        private readonly IServiceProvider _serviceProvider;

        public EmailMessageConsumer(IAzureServiceBus serviceBus, IServiceProvider serviceProvider)
        {
            _serviceBus = serviceBus;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string receivedMessage = await _serviceBus.ReceiveMessage("emailQueue");

                if (!string.IsNullOrEmpty(receivedMessage))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var emailMessageDto = JsonConvert.DeserializeObject<EmailMessageConsumerDto>(receivedMessage);

                    var message = new Message(new List<string> { emailMessageDto!.To }, emailMessageDto.From, emailMessageDto.Subject, emailMessageDto.Content);
                    await emailService.SendEmailAsync(message);
                }
            }
        }
    }
}
