using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ravendesk
{
    public class OpenAiApiClient
    {
        private readonly HttpClient _httpClient;

        public OpenAiApiClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> GetGptResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-4", 
                prompt = prompt,
                temperature = 0.7,
                max_tokens = 500
            };

            
            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/engines/davinci/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<GptResponse>();
            return responseContent?.Choices?[0].Text;
        }
    }

    public class GptResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
