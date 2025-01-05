using System.Threading.Tasks;

namespace UserService.BLL.Interfaces.Azure;

public interface IAzureServiceBus
{
    Task SendMessage(string queueName, string message);
    Task<string> ReceiveMessage(string queueName);

}