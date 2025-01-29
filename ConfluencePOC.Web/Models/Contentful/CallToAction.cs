
using ConfluencePOC.Web.Enums;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class CallToAction : ContentfulContent
{
    public static string ContentType { get; } = "callToAction";
    
    public string? Title { get; set; }

    public CallToActionSize? Size { get; set; } = CallToActionSize.Small;

    public Document? Content { get; set; }

    public string? CallToActionText { get; set; }
    
    public Page? InternalDestination { get; set; }
    
    public string? ExternalDestination { get; set; }
    
}