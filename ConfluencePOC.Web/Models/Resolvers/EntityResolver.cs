using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Configuration;

namespace ConfluencePOC.Web.Models.Resolvers;

public class EntityResolver : IContentTypeResolver
{
    public Dictionary<string, Type> _types = new Dictionary<string, Type>()
    {
        // Pages
        { Homepage.ContentType, typeof(Homepage) },
        { GeneralSupportPage.ContentType, typeof(GeneralSupportPage) },
        { ListingPage.ContentType, typeof(ListingPage) },
        { Page.ContentType, typeof(Page) },

        // Content
        { Card.ContentType, typeof(Card) },
        { ExternalAgency.ContentType, typeof(ExternalAgency) },
        { Grid.ContentType, typeof(Grid) },
        { NavigationElement.ContentType, typeof(NavigationElement) },
        { Row.ContentType, typeof(Row) },
        { RowContent.ContentType, typeof(RowContent) },
        
    };

    public Type Resolve(string contentTypeId)
    {
        return _types.TryGetValue(contentTypeId, out var type) ? type : null;
    }
}