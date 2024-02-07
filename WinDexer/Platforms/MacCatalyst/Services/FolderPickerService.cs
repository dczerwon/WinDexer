using WinDexer.Services;

namespace WinDexer.Platforms.MacCatalyst.Services;

public class FolderPickerService: IFolderPickerService
{
    public Task<string?> PickFolderAsync() => throw new NotImplementedException();
}