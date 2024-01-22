using System.Diagnostics;
using TraceTool;
using Windexer.Model.Entities;

namespace Windexer.Core.Managers;

public class IndexationManager(RootFoldersManager _rootFoldersManager, IndexEntriesManager _indexEntriesManager, DbManager _dbManager)
{
    private Dictionary<string, IndexEntry> _existingEntriesByPath;
    public static DateTime IndexationStart => _indexationStart ?? DateTime.Now;
    private static DateTime? _indexationStart;

    public Action<string,string?> OnIndexationMessage { get; set; }

    private async Task SendIndexationMessageAsync(string msgA, string? msgB = null)
    {
        TTrace.Debug.Send(msgA, msgB);
        OnIndexationMessage?.Invoke(msgA, msgB);
        await Task.Delay(10);
    }

    public async Task AddFolderToIndex(string folderPath)
    {
        TTrace.Debug.Send("Add a folder to index", folderPath);
        _rootFoldersManager.AddAsync(folderPath);
        await _dbManager.SaveChangesAsync();
        TTrace.Debug.Send("Folder to index added", folderPath);
    }

    public async Task DeleteFoldersFromIndex(List<RootFolder> folders)
    {
        if (folders == null)
            return;

        foreach (var toDelete in folders)
        {
            var related = await _indexEntriesManager.GetForRootAsync(toDelete.RootFolderId);
            if (related == null)
                continue;

            _dbManager.Delete(related);
        }
        _dbManager.Delete(folders);
        await _dbManager.SaveChangesAsync();        
    }

    public async Task ResetIndexAsync()
    {
        await _dbManager.ResetDbAsync();
    }

    public async Task RunIndexation(IEnumerable<RootFolder>? rootFolders = null)
    {
        await SendIndexationMessageAsync("Start Indexation");
        var timer = Stopwatch.StartNew();
        _indexationStart = DateTime.Now;
        if (rootFolders == null)
        {
            var result = await _rootFoldersManager.GetAsync();
            rootFolders = result.Data;
        }
        
        foreach (var rootFolder in rootFolders)
        {
            var entriesList = await _indexEntriesManager.GetForRootAsync(rootFolder.RootFolderId);
            _existingEntriesByPath = entriesList.ToDictionary(e_ => e_.RelativePath);
            foreach (var entry in _existingEntriesByPath.Values)
                _indexEntriesManager.MarkUnseen(entry);

            var indexEntry = await IndexFolder(rootFolder, null, null);
            rootFolder.StillFound = indexEntry.StillFound;
            rootFolder.IndexationDate = IndexationStart;
            await SendIndexationMessageAsync("Save indexed data");
            await _dbManager.SaveChangesAsync();
        }

        timer.Stop();
        await SendIndexationMessageAsync($"Indexation finished in {timer.Elapsed}");
    }

    private async Task<IndexEntry> IndexFolder(RootFolder root, DirectoryInfo? folder, IndexEntry? parent)
    {
//        await Task.Delay(10);

        folder ??= new DirectoryInfo(root.Path);
        await SendIndexationMessageAsync("Start folder indexation", folder.FullName);

        var relativePath = Path.GetRelativePath(root.Path, folder.FullName);
        if (_existingEntriesByPath.TryGetValue(relativePath, out var indexEntry))
            _indexEntriesManager.Update(indexEntry, folder.Exists, 0);
        else
            indexEntry = await _indexEntriesManager.AddAsync(folder, 0, root, parent);
            
        
        FileInfo[] files = [];
        try
        {
            files = folder.GetFiles();
        }
        catch (Exception e)
        {
            TTrace.Error.SendObject("Exception !", e);
        }

        if (files.Length > 0)
            await SendIndexationMessageAsync($"Index {files.Length} files");

        var count = 1;
        foreach (var file in files)
        {
            var relativeFilePath = Path.GetRelativePath(root.Path, file.FullName);
            if (_existingEntriesByPath.TryGetValue(relativeFilePath, out var existingEntry))
                _indexEntriesManager.Update(existingEntry, file.Exists, file.Length);
            else
                _indexEntriesManager.AddAsync(file, file.Length, root, indexEntry);

            indexEntry.Size += file.Length;
            indexEntry.FilesCount++;
            await SendIndexationMessageAsync($"File {count++} on {files.Length}", file.FullName);
        }

        DirectoryInfo[] subFolders = [];
        try
        {
            subFolders = folder.GetDirectories();
        }
        catch (Exception e)
        {
            TTrace.Error.SendObject("Exception !", e);
        }

        if (subFolders.Length > 0)
            await SendIndexationMessageAsync($"Index {subFolders.Length} subfolders");

        foreach (var subfolder in subFolders)
        {
            var subFolderRecord = await IndexFolder(root, subfolder, indexEntry);

            indexEntry.Size += subFolderRecord.Size;
            indexEntry.FilesCount += subFolderRecord.FilesCount;
            indexEntry.FoldersCount += subFolderRecord.FoldersCount;        
        }
        indexEntry.FoldersCount += subFolders.Length;

        await SendIndexationMessageAsync("Folder indexation finished", folder.FullName);
        return indexEntry;
    }

    public Task EnableRootFolders(List<RootFolder> rootFolders)
    {
        foreach (var rootFolder in rootFolders)
        {
            rootFolder.Enabled = true;
            _dbManager.Update(rootFolder);
        }
        return _dbManager.SaveChangesAsync();
    }

    public Task DisableRootFolders(List<RootFolder> rootFolders)
    {
        foreach (var rootFolder in rootFolders)
        {
            rootFolder.Enabled = false;
            _dbManager.Update(rootFolder);
        }
        return _dbManager.SaveChangesAsync();
    }

    public async Task PurgeRootFolders(List<RootFolder> rootFolders)
    {
        foreach (var rootFolder in rootFolders)
        {
            var toPurge = await _indexEntriesManager.GetForRootAsync(rootFolder.RootFolderId, true);
            _dbManager.Delete(toPurge);
        }
        await _dbManager.SaveChangesAsync();
    }
}
