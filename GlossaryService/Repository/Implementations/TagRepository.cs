using GlossaryService.Common.DTOs;
using GlossaryService.Common.Setting;
using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlossaryService.Repository.Implementations;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;

    public TagRepository(AppDbContext db) => _db = db;

    public async Task<PagedResultDto<GlossaryTag>> GetAllAsync(string? search, int page, int pageSize)
    {
        // ← always exclude soft deleted records
        var query = _db.GlossaryTags
            .Where(t => t.DeletedAt == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                t.Name.Contains(search) ||
                t.Slug.Contains(search));

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<GlossaryTag>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<GlossaryTag?> GetByIdAsync(Guid id) =>
        await _db.GlossaryTags
            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null); // ← exclude soft deleted

    public async Task<GlossaryTag?> GetBySlugAsync(string slug) =>
        await _db.GlossaryTags
            .FirstOrDefaultAsync(t => t.Slug == slug && t.DeletedAt == null); // ← exclude soft deleted

    public async Task<GlossaryTag> CreateAsync(GlossaryTag tag)
    {
        _db.GlossaryTags.Add(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public async Task<GlossaryTag> UpdateAsync(GlossaryTag tag)
    {
        tag.UpdatedAt = DateTime.UtcNow;
        _db.GlossaryTags.Update(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await _db.GlossaryTags.FindAsync(id);

        // ← never hard delete — stamp DeletedAt instead
        if (tag is null || tag.DeletedAt != null) return false;

        tag.DeletedAt = DateTime.UtcNow;
        tag.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsBySlugAsync(string slug) =>
        await _db.GlossaryTags
            .AnyAsync(t => t.Slug == slug && t.DeletedAt == null); // ← exclude soft deleted
}