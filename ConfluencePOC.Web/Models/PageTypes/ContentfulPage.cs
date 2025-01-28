using System.ComponentModel;
using System.Runtime.Serialization;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class ContentfulPage : IContent
{
    public SystemProperties? Sys { get; set; }
    
    public ContentfulMetadata? Metadata { get; set; }
    
    #region Page Info
    
    public string? Title { get; set; }
    
    public string? Slug { get; set; }
    
    #endregion
    
    public DateTime Fetched { get; set; } = DateTime.Now;
    
}