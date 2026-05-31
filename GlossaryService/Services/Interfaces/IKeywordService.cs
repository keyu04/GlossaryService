using GlossaryService.Common.DTOs;
using GlossaryService.DTOs;

namespace GlossaryService.Services.Interfaces;

public interface IKeywordService
{
    Task<PagedResultDto<KeywordResponseDto>> GetAllAsync(string? search, Guid? tagId, int page, int pageSize);
    Task<KeywordResponseDto?> GetByIdAsync(Guid id);
    Task<KeywordResponseDto> CreateAsync(CreateKeywordDto dto);
    Task<KeywordResponseDto?> UpdateAsync(Guid id, UpdateKeywordDto dto);
    Task<bool> DeleteAsync(Guid id);
}