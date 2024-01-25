using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using WinDexer.Core.Managers;

namespace WinDexer.Components.Shared;


public partial class IndexationStatus : ComponentBase, IDisposable
{
    [Inject] private IndexationManager IdxManager { get; set; }
    private DateTime _lastRefresh = DateTime.Now;

    protected override Task OnInitializedAsync()
    {
        IdxManager.OnCountChange = () =>
        {
            if (_lastRefresh.AddSeconds(1) >= DateTime.Now)
                return;

            _lastRefresh = DateTime.Now;
            InvokeAsync(StateHasChanged);
        };

        return base.OnInitializedAsync();
    }

    public void Dispose()
    {
        IdxManager.OnCountChange = null;
    }

    private string FilesLabel => IdxManager.FilesFound <= 1
        ? $"{IdxManager.FilesFound} file"
        : $"{IdxManager.FilesFound} files";

    private string FoldersLabel => IdxManager.FoldersFound <= 1
        ? $"{IdxManager.FoldersFound} folder"
        : $"{IdxManager.FoldersFound} folders";

    
}
