using ChatGptNet;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using OpenAI.Assistants;
using OpenAI;

namespace ravendesk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            //we can add the chatgpt api in the copilot page rather than here?
            
            builder.Services.AddChatGpt(options =>
            {
                var apiKey = "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";
                options.UseOpenAI(apiKey);
                options.DefaultModel = OpenAIChatGptModels.Gpt4;
                options.ThrowExceptionOnError = true;
                options.MessageLimit = 20;
                options.MessageExpiration = TimeSpan.FromMinutes(5);
            });
            
            
            //add community toolkit
            builder.UseMauiApp<App>();
            builder.UseMauiCommunityToolkit();

            //add fonts
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();


            //add filesaver, folderpicker
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);

            return builder.Build();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}