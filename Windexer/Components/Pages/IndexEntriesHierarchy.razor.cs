using Microsoft.AspNetCore.Components;
using Radzen;
using WinDexer.Core.Managers;
using WinDexer.Model.Entities;
using System.Linq.Dynamic.Core;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore;
using WinDexer.Core.ViewModels;

namespace WinDexer.Components.Pages;

public partial class IndexEntriesHierarchy : ComponentBase
{
    [Inject] public IndexEntriesManager IdxEntriesManager { get; set; }

    private RadzenDataGrid<IndexEntry> _datagrid;
    private IEnumerable<IndexEntry> _pageItems;
    private Dictionary<Guid, IQueryable<IndexEntry>> _children;
    private bool _isLoading = false;
    private int _itemsCount = 0;
    private LoadDataArgs _lastLoadDataArgs;

    public void RowRender(RowRenderEventArgs<IndexEntry> args)
    {
        if (string.IsNullOrEmpty(_lastLoadDataArgs?.Filter))
            args.Expandable = _children.ContainsKey(args.Data.IndexEntryId);
        else // If there is a filter, consider no row is expandable...
            args.Expandable = false;
    }

    public async Task LoadChildData(DataGridLoadChildDataEventArgs<IndexEntry> args)
    {
        var children = _children[args.Item.IndexEntryId];
        args.Data = children.ToList();

        var request = _lastLoadDataArgs?.ToRequest() ?? new FilteredListRequest();
        request.Top = 0;
        request.Skip = 0;
        var childrenIds = children.Select(e_ => e_.IndexEntryId).ToHashSet();
        var subChildren = await IdxEntriesManager.GetAsync(request, q_ => q_.Where(e_ => e_.Parent != null && childrenIds.Contains(e_.Parent.IndexEntryId)));
        foreach (var group in subChildren.Data.GroupBy(e_ => e_.Parent?.IndexEntryId ?? Guid.Empty))
            _children.Add(group.Key, group.AsQueryable());
    }

    public async Task LoadData(LoadDataArgs args)
    {
        _isLoading = true;
        
        _lastLoadDataArgs = args;
        var result = await IdxEntriesManager.GetAsync(args.ToRequest(), q_ =>
        {
            q_ = q_.Include(e_ => e_.Root);
            if (string.IsNullOrEmpty(args.Filter))
                q_ = q_.Where(e_ => e_.Parent == null);
            return q_;
        });

        if (string.IsNullOrEmpty(args.Filter))
        {
            var request = args.ToRequest();
            request.Top = 0;
            request.Skip = 0;

            var indexEntryIds = result.Data.Select(e_ => e_.IndexEntryId).ToHashSet();
            var children = await IdxEntriesManager.GetAsync(request, q_ => q_.Where(e_ => e_.Parent != null && indexEntryIds.Contains(e_.Parent.IndexEntryId)));
            _children = children.Data.GroupBy(e_ => e_.Parent?.IndexEntryId ?? Guid.Empty).ToDictionary(g_ => g_.Key, g_ => g_.AsQueryable());
        }

        _pageItems = result.Data;
        _itemsCount = result.TotalCount;

        _isLoading = false;
    }
}