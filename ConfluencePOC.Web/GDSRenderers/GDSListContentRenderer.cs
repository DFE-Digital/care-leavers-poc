using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a list.
/// </summary>
public class GDSListContentRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new ListContentRenderer.
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSListContentRenderer(ContentRendererCollection rendererCollection)
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
    /// <returns>Returns true if the content is a list, otherwise false.</returns>
    public bool SupportsContent(IContent content)
    {
        return content is List;
    }

    /// <summary>
    /// Renders the content asynchronously.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The list as a ul or ol HTML string.</returns>
    public async Task<string> RenderAsync(IContent content)
    {
        var list = content as List;
        var listTagType = "ul";
        var listClass = "govuk-list govuk-list--bullet";
        if (list.NodeType == "ordered-list")
        {
            listTagType = "ol";
            listClass = "govuk-list govuk-list--number";
        }

        var tb = new TagBuilder(listTagType);
        tb.AddCssClass(listClass);
            

        foreach (var subContent in list.Content)
        {
            var renderer = _rendererCollection.GetRendererForContent(subContent);
            tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
        }


        return tb.ToHtmlString();
    }
}