using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinDexer.Model.Entities;

[Index(nameof(Name))]
[Index(nameof(Path), IsUnique = true)]
[Index(nameof(IndexationDate))]
[Index(nameof(StillFound))]
[Index(nameof(Enabled))]
public class RootFolder : IEntity
{
    [Key]
    [Required]
    public Guid RootFolderId { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Path { get; set; } = null!;

    public DateTime? IndexationDate { get; set; }

    public bool? StillFound { get; set; }

    [Required]
    public bool Enabled { get; set; }

    public virtual List<IndexEntry> Children { get; set; }

    [NotMapped]
    public Guid Id => RootFolderId;
}

