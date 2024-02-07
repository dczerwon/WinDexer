using TraceTool;
using WinDexer.Core.ViewModels;
using WinDexer.Model.Entities;

namespace WinDexer.Core.Managers
{
    // ReSharper disable once InconsistentNaming
    public class RootFoldersManager(DbManager _dbManager)
    {
        public Task<FilteredListResponse<RootFolder>> GetAsync(FilteredListRequest? request = null, Func<IQueryable<RootFolder>, IQueryable<RootFolder>>? adaptQuery = null)
        {
            request ??= new FilteredListRequest();
            var query = _dbManager.RootFolders.AsQueryable();
            if (adaptQuery != null)
                query = adaptQuery(query);

            return _dbManager.GetAsync(query, request);
        }

        public async Task<RootFolder> AddAsync(string path)
        {
            TTrace.Debug.Send("Add root folder", path);
            var folder = new RootFolder
            {
                RootFolderId = Guid.NewGuid(),
                Name = Path.GetFileName(path),
                Path = path,
                Enabled = true,                
            };

            await _dbManager.InsertAsync(folder);
            return folder;
        }
    }
}
