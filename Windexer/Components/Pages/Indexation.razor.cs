using Microsoft.AspNetCore.Components;
using TraceTool;
using WinDexer.Core.Managers;
using WinDexer.Services;
using WinDexer.Components.Shared;
using Radzen;

namespace WinDexer.Components.Pages;

public partial class Indexation: ComponentBase, IDisposable
{
    [Inject] public IndexationManager IdxManager { get; set; } = null!;
    [Inject] public DbManager DatabaseManager { get; set; } = null!;
    [Inject] public IFolderPickerService FolderPicker { get; set; } = null!;

    private EventConsole _console = null!; 

    protected override Task OnInitializedAsync()
    {
        IdxManager.OnIndexationMessage = LogMessage;

        return base.OnInitializedAsync();
    }

    private void LogMessage(string msgA, string? msgB, MessageLevel lvl)
    {
        var message = msgA;
        if (!string.IsNullOrEmpty(msgB))
            message += ": " + msgB;

        var alertStyle = lvl switch
        {
            MessageLevel.Debug => AlertStyle.Dark,
            MessageLevel.Info => AlertStyle.Base,
            MessageLevel.Success => AlertStyle.Success,
            MessageLevel.Warning => AlertStyle.Warning,
            MessageLevel.Error => AlertStyle.Danger,
            _ => throw new NotImplementedException() // Will happen only if badly developped
        };

        _console.Log(message, alertStyle);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && IndexationManager.IsIndexing && IndexationManager.IndexationStart.HasValue)
        {
            _console.Clear();

            foreach (var message in IdxManager.ImportantMessages)
                LogMessage(message.msgA, message.msgB, message.level);

            _console.AddTruncateMessage();
        }
       
        return base.OnAfterRenderAsync(firstRender);
    }



    public async Task WipeDb()
    {
        TTrace.Debug.Send("Enter", "Home.WipeDb");
        await IdxManager.ResetIndexAsync();        
        TTrace.Debug.Send("Leave", "Home.WipeDb");
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

    public void Dispose()
    {
        IdxManager.OnIndexationMessage = null;
    }
}