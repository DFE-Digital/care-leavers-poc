
namespace ConfluencePOC.Web.Models.Contentful;

public class DefinitionLink : ContentfulContent
{
    public static string ContentType { get; } = "definitionLink";
    
    public string? Title { get; set; }
    
    public DefinitionContent? Definition { get; set; }
    

}