using System.Text;
using Azure.Messaging.ServiceBus;
using Streetcode.BLL.Interfaces.Azure;

namespace Streetcode.BLL.Services.Azure;

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
            var msgBody = Encoding.UTF8.GetString(msg.Body);

            return msgBody;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}