using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for hyperlinks to entries
/// </summary>
public class GDSEntityLinkContentRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new GDS Hyperlink Renderer
    /// </summary>
    public GDSEntityLinkContentRenderer(ContentRendererCollection rendererCollection)
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
            var et = content as EntryStructure;
            switch (et.NodeType)
            {
                case "entry-hyperlink":
                case "inline-entry-hyperlink":
                    return true;
            }

            return false;
        }

        return false;
    }

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public async Task<string> RenderAsync(IContent content)
    {
        var link = (content as EntryStructure);
        TagBuilder tb = new TagBuilder("a");
        tb.AddCssClass("govuk-hyperlink");
        switch (link.Data.Target)
        {
            case Homepage h:
                tb.Attributes["href"] = h.Slug;
                foreach (var subContent in link.Content)
                {
                    var renderer = _rendererCollection.GetRendererForContent(subContent);
                    tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
                }
                break;
            case GeneralSupportPage p:
                tb.Attributes["href"] = p.Slug;
                foreach (var subContent in link.Content)
                {
                    var renderer = _rendererCollection.GetRendererForContent(subContent);
                    tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
                }
                break;
        }

        if (tb.HasInnerHtml)
        {
            return tb.ToHtmlString();
        }
            
        return String.Empty;
    }
}