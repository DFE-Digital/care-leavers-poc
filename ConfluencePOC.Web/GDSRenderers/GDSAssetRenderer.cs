using System.Text;
using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for an asset.
/// </summary>
public class GDSAssetRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new AssetRenderer.
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSAssetRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
    }

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is an asset, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        return content is AssetStructure;
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
    /// <returns>The html img or a tag.</returns>
    public override async Task<string> RenderAsync(IContent content)
    {
        var assetStructure = content as AssetStructure;
        var asset = assetStructure.Data.Target;
        var nodeType = assetStructure.NodeType;
        var sb = new StringBuilder();
        if (nodeType != "asset-hyperlink" && asset.File?.ContentType != null &&
            asset.File.ContentType.ToLower().Contains("image"))
        {
            sb.Append($"<img class=\"full-width-image\" src=\"{asset.File.Url}\" alt=\"{asset.Title}\" />");
        }
        else
        {
            var url = asset.File?.Url;
            sb.Append(string.IsNullOrEmpty(url) ? "<a class=\"govuk-link\">" : $"<a class=\"govuk-link\" href=\"{asset.File.Url}\">");

            if (assetStructure.Content != null && assetStructure.Content.Any())
            {
                foreach (var subContent in assetStructure.Content)
                {
                    var renderer = _rendererCollection.GetRendererForContent(subContent);
                    sb.Append(await renderer.RenderAsync(subContent));
                }
            }
            else
            {
                sb.Append(asset.Title);
            }

            sb.Append("</a>");
        }

        return sb.ToString();
    }
}