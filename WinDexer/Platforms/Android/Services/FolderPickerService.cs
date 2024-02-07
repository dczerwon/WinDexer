using WinDexer.Services;

namespace WinDexer.Platforms.Android.Services;

public class FolderPickerService: IFolderPickerService
{
    public Task<string?> PickFolderAsync() => throw new NotImplementedException();
}