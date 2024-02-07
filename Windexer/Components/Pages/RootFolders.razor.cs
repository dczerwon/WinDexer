using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using TraceTool;
using WinDexer.Core.Managers;
using WinDexer.Model.Entities;
using WinDexer.Services;

namespace WinDexer.Components.Pages;

public partial class RootFolders : ComponentBase
{
    [Inject] public IndexationManager IdxManager { get; set; } = null!;
    [Inject] public DbManager DatabaseManager { get; set; } = null!;
    [Inject] public IFolderPickerService FolderPicker { get; set; } = null!;
    [Inject] public RootFoldersManager RootsManager { get; set; } = null!;

    private RadzenDataGrid<RootFolder> _datagrid = null!;

    private IEnumerable<RootFolder> _pageItems = Array.Empty<RootFolder>();
    private IList<RootFolder> _selectedRootFolders = new List<RootFolder>();
    private bool _isLoading;
    private int _itemsCount;

    private bool AllSelected =>  _pageItems.Any() && _pageItems.All(IsSelected);

    private bool IsSelected(RootFolder item)
        => _selectedRootFolders.Contains(item);

    private void ToggleRow(RootFolder item)
    {
        if (IsSelected(item))
            _selectedRootFolders.Remove(item);
        else
            _selectedRootFolders.Add(item);
    }

    private void ToggleSelectAll(bool selected)
    {                
        _selectedRootFolders = selected
            ? _pageItems.ToList()
            : new List<RootFolder>();
    }

    public async Task AddFolder()
    {
        TTrace.Debug.Send("Enter", "RootFolders.AddFolder");
        var folderPath = await FolderPicker.PickFolderAsync();
        if (string.IsNullOrEmpty(folderPath))
        {
            TTrace.Debug.Send("Leave", "RootFolders.AddFolder (No folder selected)");
            return;
        }
        
        await IdxManager.AddFolderToIndex(folderPath);        
        await _datagrid.RefreshDataAsync();
        TTrace.Debug.Send("Leave", "RootFolders.AddFolder");
    }

    private async Task LoadData(LoadDataArgs args)
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