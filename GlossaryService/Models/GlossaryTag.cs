namespace GlossaryService.Models;

public class GlossaryTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }                    // ← soft delete

    public ICollection<GlossaryKeyword> Keywords { get; set; } = new List<GlossaryKeyword>();
}

public class GlossaryKeyword
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Term { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public Guid? TagId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }                    // ← soft delete

    public GlossaryTag? Tag { get; set; }
}