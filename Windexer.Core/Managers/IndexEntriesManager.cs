using TraceTool;
using WinDexer.Model.Entities;
using WinDexer.Core.ViewModels;

namespace WinDexer.Core.Managers
{
    // ReSharper disable once InconsistentNaming
    public class IndexEntriesManager(DbManager _dbManager)
    {
        public async Task<IndexEntry> AddAsync(FileSystemInfo fsInfo, long fileSize, RootFolder root, IndexEntry? parent = null)
        {
            var fileInfo = fsInfo as FileInfo;
            var dirinfo = fsInfo as DirectoryInfo;

            TTrace.Debug.Send($"Add index entry", fsInfo.FullName);
            var entry = new IndexEntry
            {
                IndexEntryId = Guid.NewGuid(),
                Name = Path.GetFileNameWithoutExtension(fsInfo.Name),
                ContainerPath = fileInfo?.Directory?.FullName ?? dirinfo?.Parent?.FullName,
                Extension = fsInfo.Extension,
                StillFound = fsInfo.Exists,
                LastSeen = fsInfo.Exists ? IndexationManager.IndexationStart : null,
                Parent = parent,
                RelativePath = Path.GetRelativePath(root.Path, fsInfo.FullName),
                Root = root,
                IsFolder = dirinfo != null,
                Size = fileSize,
                FilesCount = 0,
                FoldersCount = 0,
                CreationTimeUtc = fsInfo.CreationTimeUtc,
                LastAccessTimeUtc = fsInfo.LastAccessTimeUtc,
                LastWriteTimeUtc = fsInfo.LastWriteTimeUtc,
            };

            await _dbManager.InsertAsync(entry);
            return entry;
        }

        public void Update(IndexEntry entry, bool stillFound, long fileSize)
        {
            TTrace.Debug.Send("Mark index entry seen", entry.RelativePath);
            entry.StillFound = stillFound;
            entry.Size = fileSize;
            entry.FoldersCount = 0;
            entry.FilesCount = 0;
            entry.LastSeen = IndexationManager.IndexationStart;

            _dbManager.Update(entry);
        }

        public void MarkUnseen(IndexEntry entry)
        {
            TTrace.Debug.Send("Mark index entry unseen", entry.RelativePath);
            entry.StillFound = false;
            _dbManager.Update(entry);
        }
        public Task<FilteredListResponse<IndexEntry>> GetAsync(FilteredListRequest? request = null, Func<IQueryable<IndexEntry>, IQueryable<IndexEntry>>? adaptQuery = null)
        {
            request ??= new FilteredListRequest();
            var query = _dbManager.IndexEntries.AsQueryable();
            if (adaptQuery != null)
                query = adaptQuery(query);

            return _dbManager.GetAsync(query, request);
        }

        public async Task<List<IndexEntry>> GetAllAsync()
        {
            var result = await GetAsync();
            return result.Data;
        }

        public async Task<List<IndexEntry>> GetForRootAsync(Guid rootFolderId, bool onlyUnseen = false)
        {
            var result = await GetAsync(adaptQuery: q_ =>
            {
                q_ = q_.Where(e_ => e_.Root.RootFolderId == rootFolderId);
                if (onlyUnseen)
                    q_ = q_.Where(e_ => !e_.StillFound);
                return q_;
            });
            return result.Data;
        }
    }
}
