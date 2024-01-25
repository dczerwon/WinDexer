using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinDexer.Model.Entities;

[Index(nameof(Name))]
[Index(nameof(RelativePath))]
[Index(nameof(IndexationDate))]
[Index(nameof(StillFound))]
[Index(nameof(IsFolder))]
[Index("RootFolderId")]
[Index("ParentIndexEntryId")]
[Index("RootFolderId", nameof(RelativePath), IsUnique = true)]
public class IndexEntry : IEntity
{
    [Key]
    [Required]
    public Guid IndexEntryId { get; set; }

    [Required]
    public string RelativePath { get; set; } = null!;

    public string? Name { get; set; }

    public string? Extension { get; set; }

    public DateTime? IndexationDate { get; set; }

    public DateTime? LastSeen { get; set; }

    [Required]
    public bool StillFound { get; set; }

    [Required]
    public bool IsFolder { get; set; }

    [Required]
    public long Size { get; set; }

    [Required]
    public int FoldersCount { get; set; }

    [Required]
    public int FilesCount { get; set; }

    [Required]
    public DateTime CreationTimeUtc { get; set; }

    [Required]
    public DateTime LastAccessTimeUtc { get; set; }

    [Required]
    public DateTime LastWriteTimeUtc { get; set; }

    [Required]
    [ForeignKey("RootFolderId")]    
    public RootFolder Root { get; set; }

    [ForeignKey("ParentIndexEntryId")]
    public IndexEntry? Parent { get; set; }

    [NotMapped]
    public string Path { get; set; } = null!;

    [NotMapped]
    public Guid Id => IndexEntryId;

    public virtual List<IndexEntry> Children { get; set; }

}
