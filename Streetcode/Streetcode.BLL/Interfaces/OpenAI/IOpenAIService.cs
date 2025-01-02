namespace Streetcode.BLL.Interfaces.OpenAI
{
    public interface IOpenAIService
    {
        Task<string> GetResponseAsync(string prompt, int maxTokens);
    }
}
