using System.Text;
using ConfluencePOC.Web.Models.Contentful;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSGridRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    /// <summary>
    /// Initializes a new PragraphRenderer
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSGridRenderer(ContentRendererCollection rendererCollection)
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
    /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
    public bool SupportsContent(IContent content)
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

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public async Task<string> RenderAsync(IContent content)
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
        
        // Render grid component
        
        switch (grid?.GridType)
        {
            case "Cards":
            case "Alternating Image and Text":

                var section = new TagBuilder("section");
                section.AddCssClass("dfe-section govuk-!-margin-top-5");
                var innerDiv = new TagBuilder("div");
                innerDiv.AddCssClass("govuk-width-container");

                if (grid.ShowTitle)
                {
                    var title = new TagBuilder("h2");
                    title.AddCssClass("govuk-heading-l");
                    if (grid.Title != null)
                    {
                        title.GenerateId(grid.Title, "-");
                        title.InnerHtml.Append(grid.Title);
                    }

                    innerDiv.InnerHtml.AppendHtml(title);
                }

                if (grid.Content != null && grid.Content.Any())
                {
                    var container = new TagBuilder("div");
                    container.AddCssClass("dfe-grid-container dfe-grid-container--wider govuk-!-margin-top-5");

                    int position = -1;

                    foreach (var gridContent in grid.Content)
                    {
                        var subContent = (Card)gridContent;
                        position++;
                        GDSCardRenderer renderer =
                            (GDSCardRenderer)_rendererCollection.GetRendererForContent(subContent);
                        renderer.Type = grid.GridType == "Alternating Image and Text"
                            ? GDSCardRenderer.CardType.AlternatingImageAndText
                            : GDSCardRenderer.CardType.Card;

                        subContent.Position = position;
                        container.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
                    }

                    innerDiv.InnerHtml.AppendHtml(container);
                }

                section.InnerHtml.AppendHtml(innerDiv);

                return section.ToHtmlString();

            case "External Links":
                StringBuilder sb = new StringBuilder();

                var h2 = new TagBuilder("h2");
                h2.AddCssClass("govuk-heading-l");
                if (grid.Title != null)
                {
                    h2.GenerateId(grid.Title, "-");
                    h2.InnerHtml.Append(grid.Title);
                }

                sb.Append(h2.ToHtmlString());

                if (grid.Content != null)
                {
                    foreach (var subContent in grid.Content)
                    {
                        var externalAgency = (ExternalAgency)content;
                        var renderer = _rendererCollection.GetRendererForContent(subContent);
                        sb.Append(await renderer.RenderAsync(subContent));
                    }
                }

                return sb.ToString();
        }

        return String.Empty;
    }
}