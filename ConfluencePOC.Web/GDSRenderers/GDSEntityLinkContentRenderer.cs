using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.AspNetCore.Authoring;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for hyperlinks to entries
/// </summary>
public class GDSEntityLinkContentRenderer : RazorContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new GDS Hyperlink Renderer
    /// </summary>
    public GDSEntityLinkContentRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ContentRendererCollection rendererCollection) : base(razorViewEngine, tempDataProvider, serviceProvider)
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
            var et = content as EntryStructure;
            switch (et.NodeType)
            {
                case "entry-hyperlink":
                case "inline-entry-hyperlink":
                    return true;
            }

            return false;
        }

        return false;
    }
    
    public override string Render(IContent content)
    {
        var result = RenderAsync(content);
        result.Wait();
        return result.Result;
    }

    /// <summary>
    /// Renders the content to an html anchor tag
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public override async Task<string> RenderAsync(IContent content)
    {
        var link = (content as EntryStructure);
        TagBuilder tb = new TagBuilder("a");
        tb.AddCssClass("govuk-hyperlink");
        switch (link.Data.Target)
        {
            case Page p:
                tb.Attributes["href"] = p.Slug;
                foreach (var subContent in link.Content)
                {
                    var renderer = _rendererCollection.GetRendererForContent(subContent);
                    tb.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
                }
                break;
        }

        if (tb.HasInnerHtml)
        {
            return tb.ToHtmlString();
        }
            
        return String.Empty;
    }
}