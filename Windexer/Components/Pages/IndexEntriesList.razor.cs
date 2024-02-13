using Microsoft.AspNetCore.Components;
using WinDexer.Core.Managers;
using WinDexer.Model.Entities;
using Radzen;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WinDexer.Components.Pages;

public partial class IndexEntriesList : ComponentBase
{
    [Inject] public IndexEntriesManager IdxEntriesManager { get; set; } = null!;

    private List<IndexEntry>? _pageItems;
    private IList<IndexEntry> _selectedItems = new List<IndexEntry>();
    private bool _isLoading;
    private int _itemsCount;

    private async Task LoadData(LoadDataArgs args)
    {
        _isLoading = true;
        
        var result = await IdxEntriesManager.GetAsync(args.ToRequest(), q_ => q_.Include(e_ => e_.Root));
        _pageItems = result.Data;
        _itemsCount = result.TotalCount;
        
        _isLoading = false;
    }

    private string AsReadableSize(long nbBytes)
    {        
        double size = nbBytes;
        var units = new [] { "B", "kB", "MB", "GB", "TB" };
        var unitIdx = 0;
        while (size > 1024 && unitIdx < units.Length)
        {
            size /= 1024;
            unitIdx++;
        }

        return $"{size:F3} {units[unitIdx]}";
    }

    public string GetIcon(IndexEntry indexEntry) => indexEntry.IsFolder ? "folder" : "description";

    public void Launch(IndexEntry indexEntry)
    {
        var pi = new ProcessStartInfo(indexEntry.Path);
        pi.UseShellExecute = true;
        Process.Start(pi);
    }

    public void ShowInExplorer(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var pi = new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"");
        pi.UseShellExecute = false;
        Process.Start(pi);
    }
}