using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class ConfigurationEntity : ContentfulContent
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
    
    public Document? Footer { get; set; }

}