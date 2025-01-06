using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.PageTypes;

public class GeneralSupportPage : ContentfulPage
{
    public static string ContentType { get; } = "generalBenefitPage";
    
    public string? Description { get; set; }
    
    public string? Type { get; set; }

    public Asset? HeroImage { get; set; }
    
    public Document? Content { get; set; }
    
    public List<IContent>? RelatedContent { get; set; }
    
    public List<IContent>? RelatedSupport { get; set; }

    
}