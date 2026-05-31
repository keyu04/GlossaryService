namespace GlossaryService.Common.Constants;

public static class MsgConst
{
    // ── Tag ──────────────────────────────────────────────────────
    public const string TagFetchAll = "Fetching all tags | Search: {Search} | Page: {Page} | PageSize: {PageSize}";
    public const string TagFetchAllDone = "Fetched {Count} tags successfully";
    public const string TagFetchById = "Fetching tag by id: {Id}";
    public const string TagNotFound = "Tag not found | Id: {Id}";
    public const string TagCreating = "Creating tag | Name: {Name} | Slug: {Slug}";
    public const string TagCreated = "Tag created successfully | Id: {Id}";
    public const string TagUpdating = "Updating tag | Id: {Id}";
    public const string TagUpdated = "Tag updated successfully | Id: {Id}";
    public const string TagDeleting = "Deleting tag | Id: {Id}";
    public const string TagDeleted = "Tag soft deleted successfully | Id: {Id}";
    public const string TagSlugConflict = "Slug already exists | Slug: {Slug}";

    // ── Keyword ──────────────────────────────────────────────────
    public const string KeywordFetchAll = "Fetching all keywords | Search: {Search} | TagId: {TagId} | Page: {Page} | PageSize: {PageSize}";
    public const string KeywordFetchAllDone = "Fetched {Count} keywords successfully";
    public const string KeywordFetchById = "Fetching keyword by id: {Id}";
    public const string KeywordNotFound = "Keyword not found | Id: {Id}";
    public const string KeywordCreating = "Creating keyword | Term: {Term}";
    public const string KeywordCreated = "Keyword created successfully | Id: {Id}";
    public const string KeywordUpdating = "Updating keyword | Id: {Id}";
    public const string KeywordUpdated = "Keyword updated successfully | Id: {Id}";
    public const string KeywordDeleting = "Deleting keyword | Id: {Id}";
    public const string KeywordDeleted = "Keyword soft deleted successfully | Id: {Id}";
    public const string KeywordTagNotFound = "Tag not found for keyword | TagId: {TagId}";
}