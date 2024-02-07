using WinDexer.Services;

namespace WinDexer.Platforms.iOS.Services;

public class FolderPickerService: IFolderPickerService
{
    public Task<string?> PickFolderAsync() => throw new NotImplementedException();
}