namespace ConfluencePOC.Web.Models;

public class CacheResult
{
    public CacheResult(string slug, DateTime? updated = null, DateTime? fetched = null, string[]? translations = null)
    {
        Slug = slug;
        Updated = updated;
        Fetched = fetched;
        Translations = translations;
    }

    public string Slug { get; }
    
    public DateTime? Updated { get; }
    
    public DateTime? Fetched { get; }
    
    public string[]? Translations { get; }

    public bool Exists => Fetched.HasValue;
}