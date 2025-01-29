using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A content renderer that renders a horizontal ruler.
/// </summary>
public class GDSHorizontalRulerContentRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    public GDSHorizontalRulerContentRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
    }
    
    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a horizontal ruler, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        return content is HorizontalRuler;
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
    /// <returns>The rendered string.</returns>
    public override Task<string> RenderAsync(IContent content)
    {
        return Task.FromResult("<hr class=\"govuk-section-break govuk-section-break--visible\">");
    }
}