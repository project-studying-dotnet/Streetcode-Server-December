using System.Text;
using Azure.Messaging.ServiceBus;
using EmailService.BLL.Interfaces.Azure;

namespace EmailService.BLL.Services.Azure;

public class AzureServiceBus(string conn) : IAzureServiceBus
{
    public async Task SendMessage(string queueName, string message)
    {
        try
        {
            var sbClientOption = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };

            var sbClient = new ServiceBusClient(conn, sbClientOption);
            var sbSender = sbClient.CreateSender(queueName);

            await sbSender.SendMessageAsync(new ServiceBusMessage(message));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> ReceiveMessage(string queueName)
    {
        try
        {
            var sbClientOption = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };
            
            var sbClient = new ServiceBusClient(conn, sbClientOption);
            var sbReceiver = sbClient.CreateReceiver(queueName);
            var msg = await sbReceiver.ReceiveMessageAsync();
            if (msg != null)
            {
                await sbReceiver.CompleteMessageAsync(msg);

                var msgBody = Encoding.UTF8.GetString(msg.Body);
                return msgBody;
            }

            return string.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}