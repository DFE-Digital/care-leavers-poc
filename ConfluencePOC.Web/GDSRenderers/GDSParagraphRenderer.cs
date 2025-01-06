using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSParagraphRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new PragraphRenderer
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSParagraphRenderer(ContentRendererCollection rendererCollection)
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
        return content is Paragraph;
    }

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public async Task<string> RenderAsync(IContent content)
    {
        var paragraph = content as Paragraph;
        var tb = new TagBuilder("p");
        tb.AddCssClass("govuk-body");

        foreach (var subContent in paragraph.Content)
        {
            var renderer = _rendererCollection.GetRendererForContent(subContent);
            tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
        }

        return tb.ToHtmlString();
    }
}