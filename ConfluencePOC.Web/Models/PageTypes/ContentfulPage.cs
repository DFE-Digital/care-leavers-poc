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
    
    public enum PageType
    {
        Guide,
        Advice
    }

    public enum PageWidth
    {
        [EnumMember(Value = "Two Thirds")]
        TwoThirds,
        [EnumMember(Value = "Full Width")]
        FullWidth
    }

    public enum HeadingType
    {
        [Obsolete("Only use H2 and below", true)]
        H1,
        H2,
        H3,
        H4,
        H5,
        H6
    }

}