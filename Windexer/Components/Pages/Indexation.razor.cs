using Microsoft.AspNetCore.Components;
using TraceTool;
using Windexer.Core.Managers;
using Windexer.Services;
using Windexer.Components.Shared;
using Radzen.Blazor;
using Radzen;

namespace Windexer.Components.Pages;

public partial class Indexation: ComponentBase
{
    [Inject] public IndexationManager IdxManager { get; set; }
    [Inject] public DbManager DatabaseManager { get; set; }
    [Inject] public IFolderPickerService FolderPicker { get; set; }

    private EventConsole _console;

    protected override Task OnInitializedAsync()
    {
        IdxManager.OnIndexationMessage = (msgA, msgB) =>
        {
            var message = msgA;
            if (!string.IsNullOrEmpty(msgB))
                message += ": " + msgB;

            _console.Log(message);
        };

        return base.OnInitializedAsync();
    }

    public async Task ResetDb()
    {
        TTrace.Debug.Send("Enter", "Home.ResetDb");
        await IdxManager.ResetIndexAsync();        
        TTrace.Debug.Send("Leave", "Home.ResetDb");
    }

    public async Task StartIndexation()
    {
        TTrace.Debug.Send("Enter", "Home.StartIndexation");
        _console.Clear();
        await IdxManager.RunIndexation();
        TTrace.Debug.Send("Leave", "Home.StartIndexation");
    }

    public async Task AddFakeData()
    {
        TTrace.Debug.Send("Enter", "Home.AddFakeData");
        await DatabaseManager.AddFakeData();
        TTrace.Debug.Send("Leave", "Home.AddFakeData");

    }
}