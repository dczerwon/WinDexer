using Microsoft.AspNetCore.Components;
using WinDexer.Core.Managers;
using WinDexer.Model.Entities;
using Radzen;
using Microsoft.EntityFrameworkCore;

namespace WinDexer.Components.Pages;

public partial class IndexEntriesList : ComponentBase
{
    [Inject] public IndexEntriesManager IdxEntriesManager { get; set; }

    private List<IndexEntry> _pageItems;
    private bool _isLoading = false;
    private int _itemsCount = 0;

    public async Task LoadData(LoadDataArgs args)
    {
        _isLoading = true;
        
        var result = await IdxEntriesManager.GetAsync(args.ToRequest(), q_ => q_.Include(e_ => e_.Root));
        _pageItems = result.Data;
        _itemsCount = result.TotalCount;
        
        _isLoading = false;
    }

    public string AsReadableSize(long size)
    {
        var units = new [] { "B", "kB", "MB", "GB", "TB" };
        var unitIdx = 0;
        while (size > 1024 && unitIdx < units.Length)
        {
            size /= 1024;
            unitIdx++;
        }

        return $"{size} {units[unitIdx]}";
    }

}