using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSCardRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    public enum CardType
    {
        Card,
        AlternatingImageAndText
    }

    public CardType Type { get; set; } = CardType.Card;


    /// <summary>
    /// Initializes a new PragraphRenderer
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSCardRenderer(ContentRendererCollection rendererCollection)
    {
        _rendererCollection = rendererCollection;
    }

    /// <summary>
    /// The order of this renderer in the collection.
    /// </summary>
    public int Order { get; set; } = 10;

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
    public bool SupportsContent(IContent content)
    {
        if (content is EntryStructure)
        {
            var structure = content as EntryStructure;
            if (structure.NodeType == "embedded-entry-block")
            {
                if (structure.Data.Target is Card)
                    return true;
            }
        }

        return content is Card;
    }

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public async Task<string> RenderAsync(IContent content)
    {
        Card card;
        if (content is Card)
        {
            card = content as Card;
        }
        else
        {
            card = (content as EntryStructure).Data.Target as Card;
        }
        
        string link = "";

        switch (card.Link)
        {
            case Homepage h:
                link = h.Slug;
                break;
            case GeneralSupportPage p:
                link = p.Slug;
                break;
        }

        var div = new TagBuilder("div");
        div.AddCssClass("hf-card");

        var anchor = new TagBuilder("a");
        anchor.Attributes.Add("href", link);

        var container = new TagBuilder("div");
        container.AddCssClass("hf-card-container");

        container.InnerHtml.AppendHtml($"<img class=\"full-width-image\" src=\"{card.Image.File.Url}\" alt=\"{card.Image.Title}\" />");
        

        container.InnerHtml.AppendHtml(
            $"<div class=\"cat-card-details\"><h3 class=\"govuk-heading-m\">{card.Title}</h3></div>");
        anchor.InnerHtml.AppendHtml(container);
        div.InnerHtml.AppendHtml(anchor);
        
        return div.ToHtmlString();
    }
}