namespace Windexer.Services;

public interface IFolderPickerService
{
    Task<string?> PickFolderAsync();
}