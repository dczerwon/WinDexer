using WinDexer.Services;

namespace WinDexer.Platforms.Tizen.Services;

public class FolderPickerService: IFolderPickerService
{
    public Task<string?> PickFolderAsync() => throw new NotImplementedException();
}