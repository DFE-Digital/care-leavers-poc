using ConfluencePOC.Web.Models.Contentful;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class ListingPage : ContentfulPage
{
    public static string ContentType { get; } = "listingPage";
    
    public string? Description { get; set; }
    
    public Document? Content { get; set; }
    
    public bool ShowEmergency { get; set; }
    
    public List<Grid>? Sections { get; set; }

    
}