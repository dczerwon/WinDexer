using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WinDexer.Model.Entities;

public class WinDexerContext : DbContext
{
    public static string DbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WinDexer.db");

    public DbSet<RootFolder> RootFolderSet { get; set; }
    public DbSet<IndexEntry> IndexEntrySet { get; set; }

    private Dictionary<Type, object> _dbSets = new Dictionary<Type, object>();

    public WinDexerContext() : base()
    {
        _dbSets.Add(typeof(RootFolder), RootFolderSet);
        _dbSets.Add(typeof(IndexEntry), IndexEntrySet);
    }

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IEntity, new()
    {
        if (_dbSets.TryGetValue(typeof(TEntity), out var dbSet))
            return dbSet as DbSet<TEntity>;

        throw new ArgumentException($"DbSet for type '{typeof(TEntity).Name}' not found.");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath};");        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RootFolder>().ToTable(nameof(RootFolder));
        modelBuilder.Entity<IndexEntry>().ToTable(nameof(IndexEntry));        
    }
}