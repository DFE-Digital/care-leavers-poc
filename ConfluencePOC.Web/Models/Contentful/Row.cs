using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class Row : ContentfulContent
{
    public static string ContentType { get; } = "row";
    
    public string Title { get; set; }
    
    public List<RowContent> Content { get; set; }

}