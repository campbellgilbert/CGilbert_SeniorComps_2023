using ChatGptNet;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Logging;

namespace ravendesk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
                builder.Services.AddChatGpt(options =>
                {
                    var apiKey = "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";
                    options.UseOpenAI(apiKey);
                    options.DefaultModel = OpenAIChatGptModels.Gpt4;
                    options.ThrowExceptionOnError = true;
                    options.MessageLimit = 20;
                    options.MessageExpiration = TimeSpan.FromMinutes(5);
                });

            /*
             private readonly HttpClient _httpClient;
            private readonly string _apiKey;

            public OpenAiAssistantCreator(string apiKey)
            {
                _httpClient = new HttpClient();
                _apiKey = apiKey;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }

            public async Task CreateAssistantAsync(string fileId)
            {
                var assistantData = new
                {
                    name = "Data visualizer",
                    description = "You are great at creating beautiful data visualizations. You analyze data present in .csv files, understand trends, and come up with data visualizations relevant to those trends. You also share a brief text summary of the trends observed.",
                    model = "gpt-4-1106-preview",
                    tools = new[] { new { type = "code_interpreter" } },
                    file_ids = new[] { fileId }
                };

                var jsonContent = JsonSerializer.Serialize(assistantData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/beta/assistants", content);
                response.EnsureSuccessStatusCode();

                // Handle the response as needed
                var responseContent = await response.Content.ReadAsStringAsync();
                // TODO: Add your logic to process the response here
            }
             */



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}