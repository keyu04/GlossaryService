using GlossaryService.Common.DTOs;
using GlossaryService.Models;

namespace GlossaryService.Repository.Interfaces;

public interface IKeywordRepository
{
    Task<PagedResultDto<GlossaryKeyword>> GetAllAsync(string? search, Guid? tagId, int page, int pageSize);
    Task<GlossaryKeyword?> GetByIdAsync(Guid id);
    Task<GlossaryKeyword> CreateAsync(GlossaryKeyword keyword);
    Task<GlossaryKeyword> UpdateAsync(GlossaryKeyword keyword);
    Task<bool> DeleteAsync(Guid id);
}