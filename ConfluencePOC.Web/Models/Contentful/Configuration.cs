using ConfluencePOC.Web.Models.PageTypes;

namespace ConfluencePOC.Web.Models.Contentful;

public class Configuration : ContentfulContent
{
    public enum BannerPhase
    {
        Alpha,
        Beta,
        Live
    }
    
    public static string ContentType { get; } = "configuration";

    
    public string ServiceName { get; set; } = String.Empty;

    public BannerPhase Phase { get; set; } = BannerPhase.Beta;
    
    public Page? HomePage { get; set; }
    
    public List<NavigationElement>? Navigation { get; set; }

}