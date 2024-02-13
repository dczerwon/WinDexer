using Microsoft.EntityFrameworkCore;
using WinDexer.Model.Entities;

namespace WinDexer.Model.Context;

public class WinDexerContext : DbContext
{
    private static string DbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RyoSaebaSan", "WinDexer");
    public static string DbPath = Path.Combine(DbFolder, "WinDexer.db");

    public DbSet<RootFolder> RootFolderSet { get; set; } = null!;
    public DbSet<IndexEntry> IndexEntrySet { get; set; } = null!;

    //private Dictionary<Type, object> _dbSets = new ();

    //public WinDexerContext() : base()
    //{
    //    _dbSets.Add(typeof(RootFolder), RootFolderSet);
    //    _dbSets.Add(typeof(IndexEntry), IndexEntrySet);
    //}

    //public DbSet<TEntity>? GetDbSet<TEntity>() where TEntity: class, IEntity, new()
    //{
    //    if (_dbSets.TryGetValue(typeof(TEntity), out var dbSet))
    //        return dbSet as DbSet<TEntity>;

    //    throw new ArgumentException($"DbSet for type '{typeof(TEntity).Name}' not found.");
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Directory.CreateDirectory(DbFolder);        
        optionsBuilder.UseSqlite($"Data Source={DbPath};");        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RootFolder>().ToTable(nameof(RootFolder));
        modelBuilder.Entity<IndexEntry>().ToTable(nameof(IndexEntry));        
    }
    
}