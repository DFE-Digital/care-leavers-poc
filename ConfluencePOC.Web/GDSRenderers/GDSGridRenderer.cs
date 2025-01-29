using ConfluencePOC.Web.Models.Contentful;
using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSGridRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;


    public GDSGridRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
        Order = 10;
    }

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
    public override bool SupportsContent(IContent content)
    {
        if (content is EntryStructure)
        {
            var structure = content as EntryStructure;
            if (structure.NodeType == "embedded-entry-block")
            {
                if (structure.Data.Target is Grid)
                    return true;
            }
        }

        return content is Grid;
    }

    public override string Render(IContent content)
    {
        var result = RenderAsync(content);
        result.Wait();
        return result.Result;
    }

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public override Task<string> RenderAsync(IContent content)
    {
        Grid grid;
        if (content is Grid)
        {
            grid = content as Grid;
        }
        else
        {
            grid = (content as EntryStructure).Data.Target as Grid;
        }

        return RenderToString("Grid/Grid", grid);
    }
}