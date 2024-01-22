using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using TraceTool;
using Windexer.Core.Managers;
using Windexer.Model.Entities;
using Windexer.Services;

namespace Windexer.Components.Pages;

public partial class RootFolders : ComponentBase
{
    [Inject] public IndexationManager IdxManager { get; set; }
    [Inject] public DbManager DatabaseManager { get; set; }
    [Inject] public IFolderPickerService FolderPicker { get; set; }
    [Inject] public RootFoldersManager RootsManager { get; set; }

    private RadzenDataGrid<RootFolder> _datagrid;

    private IEnumerable<RootFolder> _pageItems;
    private IList<RootFolder> _selectedRootFolders = new List<RootFolder>();
    private bool _isLoading = false;
    private int _itemsCount = 0;

    public bool AllSelected {
        get 
        {
            if (_pageItems == null || !_pageItems.Any())
                return false;

            return _pageItems.All(IsSelected);
        } 
    }

    public bool IsSelected(RootFolder item)
        => _selectedRootFolders.Contains(item);

    public void ToggleRow(RootFolder item)
    {
        if (IsSelected(item))
            _selectedRootFolders.Remove(item);
        else
            _selectedRootFolders.Add(item);
    }

    public void ToggleSelectAll(bool selected)
    {                
        _selectedRootFolders = selected
            ? _pageItems.ToList()
            : new List<RootFolder>();
    }

    public async Task AddFolder()
    {
        TTrace.Debug.Send("Enter", "RootFolders.AddFolder");
        var folderPath = await FolderPicker.PickFolderAsync();
        await IdxManager.AddFolderToIndex(folderPath);        
        await _datagrid.RefreshDataAsync();
        TTrace.Debug.Send("Leave", "RootFolders.AddFolder");
    }

    public async Task LoadData(LoadDataArgs args)
    {
        _isLoading = true;

        var result = await RootsManager.GetAsync(args.ToRequest());
        _pageItems = result.Data;
        _itemsCount = result.TotalCount;

        _isLoading = false;
    }

    public async Task DeleteSelected()
    {
        await IdxManager.DeleteFoldersFromIndex(_selectedRootFolders.ToList());
        await _datagrid.RefreshDataAsync();
    }

    public async Task EnableSelected() 
    {
        await IdxManager.EnableRootFolders(_selectedRootFolders.ToList());        
    }

    public async Task DisableSelected() 
    {
        await IdxManager.DisableRootFolders(_selectedRootFolders.ToList());
    }

    public async Task PurgeSelected() 
    {
        await IdxManager.PurgeRootFolders(_selectedRootFolders.ToList());
    }

    public async Task IndexSelected()
    {
        await IdxManager.RunIndexation(_selectedRootFolders);
    }
}