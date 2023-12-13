using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Media;

namespace ravendesk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            //add community toolkit
            builder.UseMauiApp<App>();
            builder.UseMauiCommunityToolkit();

            //add fonts
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "Open Sans");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("arial.ttf", "Arial");
                fonts.AddFont("COMIC.ttf", "Comic Sans");
                fonts.AddFont("Lexend-Regular.ttf", "Lexend");
                fonts.AddFont("CourierPrime-Regular.ttf", "Courier");
                fonts.AddFont("TimesNewRoman-Regular.ttf", "Times New Roman");
            });


            builder.Services.AddTransient<TextEditor>();
#if WINDOWS
            builder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);
#endif
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