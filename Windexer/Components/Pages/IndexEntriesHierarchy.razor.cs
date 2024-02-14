using Microsoft.AspNetCore.Components;
using Radzen;
using WinDexer.Core.Managers;
using WinDexer.Model.Entities;
using Microsoft.EntityFrameworkCore;
using WinDexer.Core.ViewModels;
using System.Diagnostics;

namespace WinDexer.Components.Pages;

public partial class IndexEntriesHierarchy : ComponentBase
{
    [Inject] public IndexEntriesManager IdxEntriesManager { get; set; } = null!;

    private IEnumerable<IndexEntry>? _pageItems;
    private IList<IndexEntry> _selectedItems = new List<IndexEntry>();
    private Dictionary<Guid, IQueryable<IndexEntry>> _children = new();
    private bool _isLoading;
    private int _itemsCount;
    private LoadDataArgs? _lastLoadDataArgs;

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
            _children.TryAdd(group.Key, group.AsQueryable());
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

    public string AsReadableSize(long nbBytes)
    {
        double size = nbBytes;
        var units = new[] { "B", "kB", "MB", "GB", "TB" };
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