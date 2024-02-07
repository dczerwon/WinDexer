using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using WinDexer.Core.Managers;

namespace WinDexer.Platforms.Windows.Extensions;

public static class WindowsLifecycleExtensions
{
    public static MauiAppBuilder ConfigureWindowsLifecycle(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows =>
            {                
                windows.OnWindowCreated(window =>
                {                    
                    window.Title = "WinDexer";                    
                    window.AppWindow.Closing += OnWindowClosing;
                });
            });
        });

        return builder;
    }

    private static async void OnWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        /*
            This approach ensures that the application doesn't close immediately 
            when the close button is clicked and allows the user to make a choice. 
            However, due to the asynchronous nature of the dialog, this implementation 
            might not behave as a traditional synchronous confirmation dialog found in 
            some desktop applications.         
         */

        if (!IndexationManager.IsIndexing)
            return;

        if (Application.Current == null)
            return;

        // Access the current window's XamlRoot
        var mauiWindow = Application.Current.Windows.First();
        var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow == null)
            return;

        // Cancel the closing event immediately to handle it asynchronously
        args.Cancel = true;

        // Show the confirmation dialog
        var dialog = new ContentDialog
        {
            Title = "Exit Confirmation",
            Content = $"An indexation is currently runnning.{Environment.NewLine}Do you want to exit WinDexer and cancel current indexation ?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            XamlRoot = nativeWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            // If the user confirms, close the window
            Application.Current.Quit();
        }
        // If the user does not confirm, the window remains open
    }

}
