using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace ConfluencePOC.Web.Models.Contentful;

public class NavigationElement : ContentfulContent
{
    public static string ContentType { get; } = "navigationElement";
    
    public string Title { get; set; } = string.Empty;
    
    public IContent? Link { get; set; }

    public string Slug
    {
        get
        {
            if (Link != null)
            {
                switch (Link)
                {
                    case Homepage h:
                        return h.Slug;
                    
                    case GeneralSupportPage generalSupportPage:
                        return generalSupportPage.Slug;
                    
                    case ListingPage listingPage:
                        return listingPage.Slug;
                    
                    case Page page:
                        return page.Slug;
                    
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}