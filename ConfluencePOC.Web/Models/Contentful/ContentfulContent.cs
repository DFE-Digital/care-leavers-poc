using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class ContentfulContent : IContent
{
    public SystemProperties? Sys { get; set; }
    
    public ContentfulMetadata? Metadata { get; set; }
    
    public DateTime Fetched { get; set; } = DateTime.Now;

}