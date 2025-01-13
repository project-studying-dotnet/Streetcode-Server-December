namespace EmailService.BLL.Interfaces.Azure;

public interface IAzureServiceBus
{
    Task SendMessage(string queueName, string message);
    Task<string> ReceiveMessage(string queueName);
}