namespace WinDexer.Services;

public interface IFolderPickerService
{
    Task<string?> PickFolderAsync();
}