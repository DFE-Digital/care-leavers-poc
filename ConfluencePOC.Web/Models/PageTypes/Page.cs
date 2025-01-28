using ConfluencePOC.Web.Enums;
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

    /// <summary>
    /// Whether to show the last published time at hte bottom of the page, before the footers
    /// </summary>
    public bool ShowLastUpdated { get; set; } = true;

    /// <summary>
    /// Whether to show the "Footer" before the main site footer
    /// </summary>
    public bool ShowFooter { get; set; } = true;

    /// <summary>
    /// Which levels of headings to include in the contents list
    /// </summary>
    public HeadingType[] ContentsHeadings { get; set; } = [ HeadingType.H2 ];
    
    #endregion
    
    #region Content
    
    /// <summary>
    /// The content that shows above the main body, but below the navigation
    /// </summary>
    public Document? Header { get; set; }
    
    /// <summary>
    /// The main body of content
    /// </summary>
    public Document? MainContent { get; set; }
    
    /// <summary>
    /// The right hand body of content in two-thirds width pages
    /// </summary>
    public Document? SecondaryContent { get; set; }
    
    #endregion
    
}