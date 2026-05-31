using GlossaryService.Common.Constants;
using GlossaryService.Common.DTOs;
using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Interfaces;

namespace GlossaryService.Services.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _repo;
    private readonly ILogger<TagService> _logger;

    public TagService(ITagRepository repo, ILogger<TagService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedResultDto<TagResponseDto>> GetAllAsync(string? search, int page, int pageSize)
    {
        _logger.LogInformation(MsgConst.TagFetchAll, search, page, pageSize);
        var result = await _repo.GetAllAsync(search, page, pageSize);

        _logger.LogInformation(MsgConst.TagFetchAllDone, result.TotalCount);
        return new PagedResultDto<TagResponseDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<TagResponseDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation(MsgConst.TagFetchById, id);
        var tag = await _repo.GetByIdAsync(id);

        if (tag is null)
        {
            _logger.LogWarning(MsgConst.TagNotFound, id);
            return null;
        }
        return MapToDto(tag);

    }

    public async Task<TagResponseDto> CreateAsync(CreateTagDto dto)
    {
        _logger.LogInformation(MsgConst.TagCreating, dto.Name, dto.Slug);
        if (await _repo.ExistsBySlugAsync(dto.Slug))
        {
            _logger.LogWarning(MsgConst.TagSlugConflict, dto.Slug);
            throw new InvalidOperationException($"Slug '{dto.Slug}' already exists.");
        }
        var tag = new GlossaryTag
        {
            Name = dto.Name.Trim(),
            Slug = dto.Slug.Trim().ToLower(),
            Color = dto.Color
        };

        var created = await _repo.CreateAsync(tag);
        _logger.LogInformation(MsgConst.TagCreated, created.Id);
        return MapToDto(created);
    }

    public async Task<TagResponseDto?> UpdateAsync(Guid id, UpdateTagDto dto)
    {
        _logger.LogInformation(MsgConst.TagUpdating, id);
        var tag = await _repo.GetByIdAsync(id);
        if (tag is null)
        {
            _logger.LogWarning(MsgConst.TagNotFound, id);
            return null;
        }

        tag.Name = dto.Name.Trim();
        tag.Color = dto.Color;
        tag.IsActive = dto.IsActive;

        var updated = await _repo.UpdateAsync(tag);
        _logger.LogInformation(MsgConst.TagUpdated, updated.Id);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation(MsgConst.TagDeleting, id);

        var deleted = await _repo.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(MsgConst.TagNotFound, id);
            return false;
        }

        _logger.LogInformation(MsgConst.TagDeleted, id);
        return true;
    }
    // ── private mapper ──────────────────────────────────────────
    private static TagResponseDto MapToDto(GlossaryTag t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Slug = t.Slug,
        Color = t.Color,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };
}