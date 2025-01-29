using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for an asset hyperlink.
/// </summary>
public class GDSAssetHyperlinkRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new AssetHyperlinkRenderer.
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSAssetHyperlinkRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
    }

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is an assethyperlink, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        return content is AssetHyperlink;
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
    /// <returns>The a tag.</returns>
    public override async Task<string> RenderAsync(IContent content)
    {
        var assetStructure = content as AssetHyperlink;
        var asset = assetStructure.Data.Target;
        var tb = new TagBuilder("a");
        tb.AddCssClass("govuk-link");

        var url = asset.File?.Url;
        if (!string.IsNullOrEmpty(url))
        {
            tb.Attributes.Add("href", asset.File.Url);
        }

        if (assetStructure.Content != null && assetStructure.Content.Any())
        {
            foreach (var subContent in assetStructure.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
            }
        }
        else
        {
            tb.InnerHtml.Append(asset.Title);
        }

        return tb.ToHtmlString();
    }
}