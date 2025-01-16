using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class Page : ContentfulPage
{
    # region Page Info
    
    /// <summary>
    /// The content type identifier from Contentful
    /// </summary>
    public static string ContentType { get; } = "page";

    /// <summary>
    /// A link to a parent page, if applicable
    /// </summary>
    public Page? Parent { get; set; }
    
    #endregion
    
    #region SEO
    
    /// <summary>
    /// The title of the page, used for search engines and sharing
    /// </summary>
    public string? SeoTitle { get; set; }
    
    /// <summary>
    /// The description for search engines and sharing the page
    /// </summary>
    public string? SeoDescription { get; set; }
    
    /// <summary>
    /// The image to show to search engines and when sharing pages
    /// </summary>
    public Asset? SeoImage { get; set; }
    
    #endregion
    
    #region Layout

    /// <summary>
    /// The width for the main content of the page - Full width is normally used for things like the homepage
    /// </summary>
    public PageWidth Width { get; set; } = PageWidth.TwoThirds;

    /// <summary>
    /// The type of this page, if applicable (eg: Guide, Article, Blog Entry, Homepage)
    /// </summary>
    public PageType? Type { get; set; }

    /// <summary>
    /// Whether to show a "Contents" block for the main content
    /// </summary>
    public bool ShowContentsBlock { get; set; } = true;

    public bool ShowLastUpdated { get; set; } = true;

    /// <summary>
    /// Which levels of headings to include in the contents list
    /// </summary>
    public HeadingType[] ContentsHeadings { get; set; } = [ HeadingType.H2 ];
    
    #endregion
    
    #region Content
    
    public Document? Header { get; set; }
    
    public Document? MainContent { get; set; }
    
    public Document? SecondaryContent { get; set; }
    
    public Document? Footer { get; set; }
    
    #endregion
    
}