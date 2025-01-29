using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a list.
/// </summary>
public class GDSListContentRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new ListContentRenderer.
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSListContentRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
    }

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a list, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        return content is List;
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
    /// <returns>The list as a ul or ol HTML string.</returns>
    public override async Task<string> RenderAsync(IContent content)
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