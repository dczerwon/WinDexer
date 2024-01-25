using Microsoft.Extensions.Logging;
using Radzen;
using TraceTool;
using WinDexer.Core;
using WinDexer.Services;

namespace WinDexer
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
                .ConfigureWindowsLifecycle()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services
                .AddWinDexerManagers()
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
