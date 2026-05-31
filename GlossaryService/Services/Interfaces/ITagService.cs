using GlossaryService.Common.DTOs;
using GlossaryService.DTOs;

namespace GlossaryService.Services.Interfaces;

public interface ITagService
{
    Task<PagedResultDto<TagResponseDto>> GetAllAsync(string? search, int page, int pageSize);
    Task<TagResponseDto?> GetByIdAsync(Guid id);
    Task<TagResponseDto> CreateAsync(CreateTagDto dto);
    Task<TagResponseDto?> UpdateAsync(Guid id, UpdateTagDto dto);
    Task<bool> DeleteAsync(Guid id);
}