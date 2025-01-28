using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Configuration;

namespace ConfluencePOC.Web.Models.Resolvers;

public class EntityResolver : IContentTypeResolver
{
    public Dictionary<string, Type> _types = new Dictionary<string, Type>()
    {
        // Configuration
        { ConfigurationEntity.ContentType, typeof(ConfigurationEntity) },
        { NavigationElement.ContentType, typeof(NavigationElement) },

        // Pages
        { Page.ContentType, typeof(Page) },

        // Content
        { CallToAction.ContentType, typeof(CallToAction) },
        { Card.ContentType, typeof(Card) },
        { DefinitionBlock.ContentType, typeof(DefinitionBlock) },
        { DefinitionContent.ContentType, typeof(DefinitionContent) },
        { DefinitionLink.ContentType, typeof(DefinitionLink) },
        { ExternalAgency.ContentType, typeof(ExternalAgency) },
        { Grid.ContentType, typeof(Grid) },
        { RichContent.ContentType, typeof(RichContent) },
        { RichContentBlock.ContentType, typeof(RichContentBlock) },
        
    };

    public Type Resolve(string contentTypeId)
    {
        return _types.TryGetValue(contentTypeId, out var type) ? type : null;
    }
}