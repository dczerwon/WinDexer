using Microsoft.AspNetCore.Components;
using Windexer.Core.Managers;
using Windexer.Model.Entities;
using Radzen;
using Microsoft.EntityFrameworkCore;

namespace Windexer.Components.Pages;

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

}