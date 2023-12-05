﻿using ChatGptNet;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using OpenAI.Assistants;
using OpenAI;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;

namespace ravendesk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            //we can add the chatgpt api in the copilot page rather than here?

            //CITE: https://github.com/RageAgainstThePixel/OpenAI-DotNet 

            //add custom rend capabilities
            
                //XamarinCustomRenderer.iOS.Renderers.PressableViewRenderer));


            //add community toolkit
            builder.UseMauiApp<App>();
            builder.UseMauiCommunityToolkit();

            //add fonts
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
                .ConfigureMauiHandlers((handlers) =>
                {
                    handlers.AddHandler(typeof(Editor), typeof(EditorRenderer));
                    //handlers.AddHandler(typeof(TextEditor), typeof(EditorRenderer));
                });

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