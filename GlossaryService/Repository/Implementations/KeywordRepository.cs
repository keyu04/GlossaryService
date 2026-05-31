using GlossaryService.Common.DTOs;
using GlossaryService.Common.Setting;
using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlossaryService.Repository.Implementations;

public class KeywordRepository : IKeywordRepository
{
    private readonly AppDbContext _db;

    public KeywordRepository(AppDbContext db) => _db = db;

    public async Task<PagedResultDto<GlossaryKeyword>> GetAllAsync(string? search, Guid? tagId, int page, int pageSize)
    {
        // ← always exclude soft deleted records
        var query = _db.GlossaryKeywords
            .Include(k => k.Tag)
            .Where(k => k.DeletedAt == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(k =>
                k.Term.Contains(search) ||
                k.Synonyms.Contains(search));

        if (tagId.HasValue)
            query = query.Where(k => k.TagId == tagId);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(k => k.Term)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<GlossaryKeyword>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<GlossaryKeyword?> GetByIdAsync(Guid id) => await _db.GlossaryKeywords.Include(k => k.Tag)
            .FirstOrDefaultAsync(k => k.Id == id && k.DeletedAt == null); // ← exclude soft deleted

    public async Task<GlossaryKeyword> CreateAsync(GlossaryKeyword keyword)
    {
        _db.GlossaryKeywords.Add(keyword);
        await _db.SaveChangesAsync();
        return keyword;
    }

    public async Task<GlossaryKeyword> UpdateAsync(GlossaryKeyword keyword)
    {
        keyword.UpdatedAt = DateTime.UtcNow;
        _db.GlossaryKeywords.Update(keyword);
        await _db.SaveChangesAsync();
        return keyword;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var keyword = await _db.GlossaryKeywords.FindAsync(id);
        // ← never hard delete — stamp DeletedAt instead
        if (keyword is null || keyword.DeletedAt != null) return false;

        keyword.DeletedAt = DateTime.UtcNow;
        keyword.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}