using ConfluencePOC.Web.Models.PageTypes;

namespace ConfluencePOC.Web.Models.Contentful;

public class NavigationElement : ContentfulContent
{
    public static string ContentType { get; } = "navigationElement";
    
    public string Title { get; set; } = string.Empty;
    
    public Page? Link { get; set; }

    public string Slug => Link?.Slug ?? string.Empty;
}