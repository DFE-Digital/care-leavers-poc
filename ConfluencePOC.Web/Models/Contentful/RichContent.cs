
using ConfluencePOC.Web.Enums;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class RichContent : ContentfulContent
{
    public static string ContentType { get; } = "richContent";
    
    public string? Description { get; set; }
    
    public BackgroundColour? Background { get; set; }
    
    public ContentWidth? Width { get; set; }
    
    public Document? Content { get; set; }

}