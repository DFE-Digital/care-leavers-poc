using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class ContentfulPage : IContent
{
    public SystemProperties? Sys { get; set; }
    
    public ContentfulMetadata? Metadata { get; set; }
    
    public string? Title { get; set; }
    
    public string? Slug { get; set; }
    
    public DateTime Fetched { get; set; } = DateTime.Now;

}