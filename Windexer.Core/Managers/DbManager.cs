using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TraceTool;
using WinDexer.Core.ViewModels;
using WinDexer.Model.Entities;
using System.Linq.Dynamic.Core;
using WinDexer.Model.Context;

namespace WinDexer.Core.Managers;

public class DbManager
{
    private readonly WinDexerContext _context;

    public DbManager(WinDexerContext context)
    {
        _context = context;
        _context.Database.Migrate();
    }

    public async Task<FilteredListResponse<TEntity>> GetAsync<TEntity>(IQueryable<TEntity> query, FilteredListRequest request) where TEntity: class, IEntity, new()
    {
        if (!string.IsNullOrEmpty(request.Filter))
            query = query.Where(request.Filter);

        var count = await query.CountAsync();

        query = request.ApplyOrderBy(query).Skip(request.Skip);
        if (request.Top > 0)
            query = query.Take(request.Top);

        var result = await query.ToListAsync();
        return new FilteredListResponse<TEntity>()
        {
            Data = result,
            TotalCount = count
        };
    }

    public async Task InsertAsync(IEntity record) 
        => await _context.AddAsync(record);

    public void Update(IEntity record)
        => _context.Update(record);

    public void Delete(IEntity record)
        => _context.Remove(record);

    public void Delete(IEnumerable<IEntity> records)
        => _context.RemoveRange(records);

    public async Task<bool> SaveChangesAsync(Func<Exception, Task>? errorHandler = null)
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            if (errorHandler != null)
                await errorHandler.Invoke(ex);
            else
            {
                TTrace.Error.SendObject("Exception", ex);
                TTrace.Error.SendStack("Call Stack");
                throw;
            }               

            return false;
        }
    }

    public DbSet<RootFolder> RootFolders
        => _context.RootFolderSet;


    public DbSet<IndexEntry> IndexEntries
        => _context.IndexEntrySet;

    public async Task ResetDbAsync()
    {
        await _context.Database.CloseConnectionAsync();
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.MigrateAsync();
    }

    public async Task AddFakeData()
    {
        var watcher = Stopwatch.StartNew();
        for (int i = 1; i <= 1000; i++)
        {
            var rootFolder = new RootFolder
            {
                Name = $"Fake Root Folder {i:D4}",
                Enabled = true,
                Path = @$"C:\FakeRootFolder_{i:D4}",
                RootFolderId = Guid.NewGuid(),
            };

            TTrace.Debug.Send("Add Fake Root", i.ToString("D4"));
            await InsertAsync(rootFolder);

            var rootIndexEntry = new IndexEntry
            {
                IndexEntryId = Guid.NewGuid(),
                Name = ".",
                StillFound = true,
                LastSeen = DateTime.Now,
                Parent = null,
                RelativePath = ".",
                Root = rootFolder,
                IsFolder = false,
                Size = 0,
                FilesCount = 0,
                FoldersCount = 0,
                CreationTimeUtc = DateTime.Now,
                LastAccessTimeUtc = DateTime.Now,
                LastWriteTimeUtc = DateTime.Now,
            };
            await InsertAsync(rootIndexEntry);

            for (int j = 2; j <= 1000; j++)
            {
                var entry = new IndexEntry
                {
                    IndexEntryId = Guid.NewGuid(),
                    Name = $"Fake Index Entry {j:D4}",
                    Extension = ".bla",
                    StillFound = true,
                    LastSeen = DateTime.Now,
                    Parent = rootIndexEntry,
                    RelativePath = $"Fake Index Entry {j:D4}.bla",
                    Root = rootFolder,
                    IsFolder = false,
                    Size = 0,
                    FilesCount = 0,
                    FoldersCount = 0,
                    CreationTimeUtc = DateTime.Now,
                    LastAccessTimeUtc = DateTime.Now,
                    LastWriteTimeUtc = DateTime.Now,
                };                
                await InsertAsync(entry);
            }
        }       

        await SaveChangesAsync();
        watcher.Stop();
        TTrace.Debug.Send("Fake data added", $"{watcher.ElapsedMilliseconds} ms");
    }
}