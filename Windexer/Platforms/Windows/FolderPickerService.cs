using Windows.Storage.Pickers;

namespace Windexer.Services;

public class FolderPickerService: IFolderPickerService
{
    public async Task<string?> PickFolderAsync()
    {
        // https://github.com/microsoft/CsWinRT/blob/master/docs/interop.md#windows-sdk
        var picker = new FolderPicker();

        var window = new Microsoft.UI.Xaml.Window();        
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        var result = await picker.PickSingleFolderAsync();
        return result?.Path;
    }
}