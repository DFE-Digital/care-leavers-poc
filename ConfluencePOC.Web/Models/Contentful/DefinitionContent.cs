
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class DefinitionContent : ContentfulContent
{
    public static string ContentType { get; } = "definition";
    
    public string? Title { get; set; }
    
    public Document? Definition { get; set; }
    
    public IContent? Page { get; set; }

}