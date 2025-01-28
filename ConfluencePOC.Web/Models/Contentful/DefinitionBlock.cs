
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class DefinitionBlock : ContentfulContent
{
    public static string ContentType { get; } = "definitionBlock";
    
    public string? Title { get; set; }
    
    public DefinitionContent? Definition { get; set; }
    
    public bool ShowTitle { get; set; }

}