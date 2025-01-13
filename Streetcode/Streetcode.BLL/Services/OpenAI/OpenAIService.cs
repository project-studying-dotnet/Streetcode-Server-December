using Streetcode.BLL.Interfaces.OpenAI;
using System.Text;

namespace Streetcode.BLL.Services.OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIService(IHttpClientFactory httpClientFactory, string apiKey)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI_Client");
            _apiKey = apiKey;
        }

        public async Task<string> GetResponseAsync(string prompt, int maxTokens)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                store = true,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = maxTokens
            };

            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Headers = { { "Authorization", $"Bearer {_apiKey}" } },
                Content = jsonContent
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error on request to OpenAI API: {response.StatusCode}\nDetails: {errorResponse}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
