using Microsoft.Extensions.Logging;
using Radzen;
using TraceTool;
using Windexer.Core;
using Windexer.Services;

namespace Windexer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            TTrace.Options.SendMode = SendMode.Socket;
            TTrace.Options.SocketHost = "127.0.0.1";
            TTrace.Options.SocketPort = 8090;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services
                .AddWindexerManagers()
                .AddSingleton<IFolderPickerService, FolderPickerService>()
                .AddRadzenComponents()
                .AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
