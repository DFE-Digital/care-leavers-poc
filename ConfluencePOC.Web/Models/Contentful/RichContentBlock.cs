
namespace ConfluencePOC.Web.Models.Contentful;

public class RichContentBlock : ContentfulContent
{
    public static string ContentType { get; } = "richContentBlock";
    
    public string? Title { get; set; }
    
    public List<RichContent?>? Entries { get; set; }

}