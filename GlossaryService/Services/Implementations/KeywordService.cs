using GlossaryService.Common.Constants;
using GlossaryService.Common.DTOs;
using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Interfaces;

namespace GlossaryService.Services.Implementations;

public class KeywordService : IKeywordService
{
    private readonly IKeywordRepository _repo;
    private readonly ITagRepository _tagRepo;

    private readonly ILogger<KeywordService> _logger;

    public KeywordService(IKeywordRepository repo, ITagRepository tagRepo, ILogger<KeywordService> logger)
    {
        _repo = repo;
        _tagRepo = tagRepo;
        _logger = logger;
    }

    public async Task<PagedResultDto<KeywordResponseDto>> GetAllAsync(string? search, Guid? tagId, int page, int pageSize)
    {

        _logger.LogInformation(MsgConst.KeywordFetchAll, search, tagId, page, pageSize);
        var result = await _repo.GetAllAsync(search, tagId, page, pageSize);
        _logger.LogInformation(MsgConst.KeywordFetchAllDone, result.TotalCount);

        return new PagedResultDto<KeywordResponseDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<KeywordResponseDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation(MsgConst.KeywordFetchById, id);
        var keyword = await _repo.GetByIdAsync(id);

        if (keyword is null)
        {
            _logger.LogWarning(MsgConst.KeywordNotFound, id);
            return null;
        }
        return MapToDto(keyword);
    }

    public async Task<KeywordResponseDto> CreateAsync(CreateKeywordDto dto)
    {
        // ── If TagId provided, verify that tag actually exists ──
        _logger.LogInformation(MsgConst.KeywordCreating, dto.Term);
        if (dto.TagId.HasValue)
        {
            var tagExists = await _tagRepo.GetByIdAsync(dto.TagId.Value);
            if (tagExists is null)
            {
                _logger.LogWarning(MsgConst.TagNotFound, dto.TagId);
                throw new KeyNotFoundException($"Tag with id '{dto.TagId}' not found.");
            }
        }

        var keyword = new GlossaryKeyword
        {
            Term = dto.Term.Trim(),
            Synonyms = dto.Synonyms.Trim(),
            TagId = dto.TagId
        };

        var created = await _repo.CreateAsync(keyword);
        _logger.LogInformation(MsgConst.KeywordCreated, created.Id);
        // ── Reload with Tag navigation for response ──
        var withTag = await _repo.GetByIdAsync(created.Id);
        return MapToDto(withTag!);
    }

    public async Task<KeywordResponseDto?> UpdateAsync(Guid id, UpdateKeywordDto dto)
    {
        _logger.LogInformation(MsgConst.KeywordUpdating, id);
        var keyword = await _repo.GetByIdAsync(id);
        if (keyword is null)
        {
            _logger.LogWarning(MsgConst.KeywordNotFound, id);
            return null;
        }


        // ── If TagId changed, verify new tag exists ──
        if (dto.TagId.HasValue)
        {
            var tagExists = await _tagRepo.GetByIdAsync(dto.TagId.Value);
            if (tagExists is null)
            {
                _logger.LogWarning(MsgConst.KeywordTagNotFound, dto.TagId);
                throw new KeyNotFoundException($"Tag with id '{dto.TagId}' not found.");
            }
        }

        keyword.Term = dto.Term.Trim();
        keyword.Synonyms = dto.Synonyms.Trim();
        keyword.TagId = dto.TagId;
        keyword.IsActive = dto.IsActive;

        var updated = await _repo.UpdateAsync(keyword);

        // ── Reload with Tag navigation for response ──
        var withTag = await _repo.GetByIdAsync(updated.Id);
        return MapToDto(withTag!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation(MsgConst.KeywordDeleting, id);

        var deleted = await _repo.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(MsgConst.KeywordNotFound, id);
            return false;
        }

        _logger.LogInformation(MsgConst.KeywordDeleted, id);
        return true;
    }

    // ── Private mapper ───────────────────────────────────────────
    private static KeywordResponseDto MapToDto(GlossaryKeyword k) => new()
    {
        Id = k.Id,
        Term = k.Term,
        Synonyms = k.Synonyms,
        TagId = k.TagId,
        TagName = k.Tag?.Name,       // ← flattened from navigation property
        IsActive = k.IsActive,
        CreatedAt = k.CreatedAt
    };
}