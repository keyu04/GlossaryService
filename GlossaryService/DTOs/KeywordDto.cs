using System.ComponentModel.DataAnnotations;

namespace GlossaryService.DTOs;

public class CreateKeywordDto
{
    [Required(ErrorMessage = "Term is required.")]
    public string Term { get; set; } = string.Empty;
    [Required(ErrorMessage = "Synonyms are required.")]
    public string Synonyms { get; set; } = string.Empty;  // "tamatar, tomate"
    public Guid? TagId { get; set; }
}

public class UpdateKeywordDto
{
    [Required(ErrorMessage = "Term is required.")]
    public string Term { get; set; } = string.Empty;
    [Required(ErrorMessage = "Synonyms are required.")]
    public string Synonyms { get; set; } = string.Empty;
    public Guid? TagId { get; set; }
    public bool IsActive { get; set; }
}

public class KeywordResponseDto
{
    public Guid Id { get; set; }
    public string Term { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public Guid? TagId { get; set; }
    public string? TagName { get; set; }   // flattened from navigation
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
