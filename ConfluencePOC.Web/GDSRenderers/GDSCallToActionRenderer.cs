using ConfluencePOC.Web.Models.Contentful;
using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSCallToActionRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;


    public GDSCallToActionRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
    {
        _rendererCollection = rendererCollection;
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
                if (structure.Data.Target is CallToAction)
                    return true;
            }
        }

        return content is CallToAction;
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
        CallToAction? block;
        if (content is CallToAction)
        {
            block = content as CallToAction;
        }
        else
        {
            block = (content as EntryStructure)?.Data.Target as CallToAction;
        }

        return RenderToString("CallToAction/CallToAction", block);
    }
}