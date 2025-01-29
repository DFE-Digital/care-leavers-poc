using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a hyperlink.
/// </summary>
public class GDSHyperlinkContentRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new HyperlinkContentRenderer.
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSHyperlinkContentRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
    }
    
    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a hyperlink, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        return content is Hyperlink;
    }
    
    public override string Render(IContent content)
    {
        var result = RenderAsync(content);
        result.Wait();
        return result.Result;
    }

    /// <summary>
    /// Renders the content asynchronously.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The a tag as a string.</returns>
    public override async Task<string> RenderAsync(IContent content)
    {
        var link = content as Hyperlink;
        var tb = new TagBuilder("a");
        tb.AddCssClass("govuk-link");
        tb.Attributes.Add("href", link.Data.Uri);
        tb.Attributes.Add("title", link.Data.Title);
            

        foreach (var subContent in link.Content)
        {
            var renderer = _rendererCollection.GetRendererForContent(subContent);
            tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
        }

        return tb.ToHtmlString();
    }
}