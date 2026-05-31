using GlossaryService.Common.DTOs;
using GlossaryService.Models;

namespace GlossaryService.Repository.Interfaces;

public interface ITagRepository
{
    Task<PagedResultDto<GlossaryTag>> GetAllAsync(string? search, int page, int pageSize);
    Task<GlossaryTag?> GetByIdAsync(Guid id);
    Task<GlossaryTag?> GetBySlugAsync(string slug);
    Task<GlossaryTag> CreateAsync(GlossaryTag tag);
    Task<GlossaryTag> UpdateAsync(GlossaryTag tag);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsBySlugAsync(string slug);
}