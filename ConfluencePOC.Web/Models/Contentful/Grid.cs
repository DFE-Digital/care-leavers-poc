using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class Grid : ContentfulContent
{
    public static string ContentType { get; } = "grid";
    
    public string? Title { get; set; }
    
    public string? GridType { get; set; }
    
    public bool ShowTitle { get; set; }
    
    public List<IContent>? Content { get; set; }

    public string? CssClass { get; set; } = "govuk-grid-column-full";

}