using System.ComponentModel.DataAnnotations;

namespace GlossaryService.DTOs;

public class CreateTagDto
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Slug is required.")]
    [MaxLength(100, ErrorMessage = "Slug cannot exceed 80 characters.")]
    public string Slug { get; set; } = string.Empty;

    public string? Color { get; set; }
}

public class UpdateTagDto
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Color is required.")]
    public string? Color { get; set; }
    public bool IsActive { get; set; }
}

public class TagResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}