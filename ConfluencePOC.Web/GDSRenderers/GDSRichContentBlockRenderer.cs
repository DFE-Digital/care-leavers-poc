using ConfluencePOC.Web.Models.Contentful;
using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSRichContentBlockRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;


    public GDSRichContentBlockRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
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
                if (structure.Data.Target is RichContentBlock)
                    return true;
            }
        }

        return content is RichContentBlock;
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
        RichContentBlock? block;
        if (content is RichContentBlock)
        {
            block = content as RichContentBlock;
        }
        else
        {
            block = (content as EntryStructure)?.Data.Target as RichContentBlock;
        }

        return RenderToString("RichContent/Block", block);
    }
}