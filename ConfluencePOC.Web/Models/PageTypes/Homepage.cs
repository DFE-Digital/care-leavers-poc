using ConfluencePOC.Web.Models.Contentful;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class Homepage : ContentfulPage
{
    public static string ContentType { get; } = "homepage";
    
    public string? Tagline { get; set; }
    
    public string? MetaTitle { get; set; }

    public Asset? HeroImage { get; set; }
    
    public List<Row>? Content { get; set; }

    
    public List<NavigationElement>? Navigation { get; set; }


}